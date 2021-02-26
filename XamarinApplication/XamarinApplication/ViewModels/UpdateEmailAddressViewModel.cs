using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class UpdateEmailAddressViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Addresses _address;
        #endregion

        #region Constructors
        public UpdateEmailAddressViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public Addresses Addresses
        {
            get { return _address; }
            set
            {
                _address = value;
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
        public async void EditAddress()
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
            var emailPattern = "^[a-z0-9._-]+@[a-z0-9._-]+\\.[a-z]{2,6}$";
            if (string.IsNullOrEmpty(Addresses.addressMail))
            {
                Value = true;
                return;
            }
            if (!String.IsNullOrWhiteSpace(Addresses.addressMail) && !(Regex.IsMatch(Addresses.addressMail, emailPattern)))
            {
                Value = true;
                return;
            }
            var address = new Addresses
            {
                id = Addresses.id,
                code = Addresses.code,
                addressMail = Addresses.addressMail
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<Addresses>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/mailSender/updateAddress",
            res,
            address);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Email Updated");
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
                    EditAddress();
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
