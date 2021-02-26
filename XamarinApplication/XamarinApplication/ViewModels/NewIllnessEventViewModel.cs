using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class NewIllnessEventViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewIllnessEventViewModel()
        {
            apiService = new ApiServices();

        }
        #endregion

        #region Properties
        public string Code { get; set; }
        public string Description { get; set; }

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
        public async void NewIllnessEvent()
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
            if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(Code))
            {
                HasError = true;
                return;
            }
            else
            {
                HasError = false;
            }
            var _illnessEvent = new AddIllnessEvent
            {
                code = Code,
                description = Description
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddIllnessEvent>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/illnessEvent/save",
            res,
            _illnessEvent);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            HasError = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Illness Event Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Save
        {
            get
            {
                return new Command(() =>
                {
                    NewIllnessEvent();
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
                });
            }
        }
        #endregion
    }
}
