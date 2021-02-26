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
    public class NewICDOViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewICDOViewModel()
        {
            apiService = new ApiServices();

        }
        #endregion

        #region Properties
        public string Code { get; set; }
        public string Description { get; set; }

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
        public async void AddNewICDO()
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
            if (string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(Description))
            {
                Value = true;
                return;
            }
            var _icdo = new AddIcdo
            {
                code = Code,
                description = Description
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddIcdo>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/icdo/save",
            res,
            _icdo);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "ICDO Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand SaveICDO
        {
            get
            {
                return new Command(() =>
                {
                    AddNewICDO();
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
