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
    public class UpdateICDOViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Icdo _icdo;
        #endregion

        #region Constructors
        public UpdateICDOViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public Icdo Icdo
        {
            get { return _icdo; }
            set
            {
                _icdo = value;
                OnPropertyChanged();
            }
        }
        private bool value = false;
        public bool Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void EditICDO()
        {
            Value = true;
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            if (string.IsNullOrEmpty(Icdo.code) || string.IsNullOrEmpty(Icdo.description))
            {
                Value = true;
                return;
            }
            var icdo = new Icdo
            {
                id = Icdo.id,
                code = Icdo.code,
                description = Icdo.description
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<Icdo>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/icdo/update",
            res,
            icdo);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            ICDOViewModel.GetInstance().Update(icdo);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "ICDO Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand UpdateICDO
        {
            get
            {
                return new Command(() =>
                {
                    EditICDO();
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
