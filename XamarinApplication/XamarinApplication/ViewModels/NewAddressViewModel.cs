using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class NewAddressViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        public string _email;
        #endregion

        #region Constructor
        public NewAddressViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public string Email
        {
            get { return _email; }
            set
            {
                this._email = value;
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
        public async void NewAddress()
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
            if (string.IsNullOrEmpty(Email))
            {
                Value = true;
                return;
            }
            if (!String.IsNullOrWhiteSpace(Email) && !(Regex.IsMatch(Email, emailPattern)))
            {
                Value = true;
                return;
            }  

            List<AddAddress> addAddresses = new List<AddAddress>();
            addAddresses.Add(new AddAddress()
            {
                code = "#MailSender",
                addressMail = Email,
                duplicate = false
            });
            var _jobCron = new AddJobCron
            {
                addresses = addAddresses
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddJobCron>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/mailSender/save",
            res,
            _jobCron);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Email Added");
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
                    NewAddress();
                    /*  Debug.WriteLine("********Days*************");
                      Debug.WriteLine(Days);
                      Debug.WriteLine("********Hour*************");
                      Debug.WriteLine(Hour);
                      Debug.WriteLine("********Minute*************");
                      Debug.WriteLine(Minute);*/
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
