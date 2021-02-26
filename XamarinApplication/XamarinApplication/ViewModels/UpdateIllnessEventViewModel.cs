using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class UpdateIllnessEventViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private IllnessEvent _icdo;
        #endregion

        #region Constructors
        public UpdateIllnessEventViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public IllnessEvent IllnessEvent
        {
            get { return _icdo; }
            set
            {
                _icdo = value;
                OnPropertyChanged();
            }
        }
        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set
            {
                this._hasError = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void EditIllnessEvent()
        {
            HasError = true;
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            if (string.IsNullOrEmpty(IllnessEvent.code) || string.IsNullOrEmpty(IllnessEvent.description))
            {
                HasError = true;
                return;
            }
            else
            {
                HasError = false;
            }
            var icdo = new IllnessEvent
            {
                id = IllnessEvent.id,
                code = IllnessEvent.code,
                description = IllnessEvent.description
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<IllnessEvent>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/illnessEvent/update",
            res,
            icdo);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            HasError = false;
            IllnessEventViewModel.GetInstance().Update(icdo);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Illness Event Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Update
        {
            get
            {
                return new Command(() =>
                {
                    EditIllnessEvent();
                });
            }
        }
        public Command ClosePopup
        {
            get
            {
                return new Command(() =>
                {
                    Navigation.PopPopupAsync();
                    //Navigation.PopAsync();
                    Debug.WriteLine("********Close*************");
                });
            }
        }
        #endregion
    }
}
