using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class ForgotPasswordViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        public string _email;
        #endregion

        #region Constructor
        public ForgotPasswordViewModel()
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

            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/login/resetPassword?email="+ Email;
            Debug.WriteLine("********url*************");
            Debug.WriteLine(url);
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                return;
            }
            var result = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("********result*************");
            Debug.WriteLine(result);
            Value = false;
            DependencyService.Get<INotification>().CreateNotification(Languages.Notification, Languages.CkeckEmail);
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Send
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
