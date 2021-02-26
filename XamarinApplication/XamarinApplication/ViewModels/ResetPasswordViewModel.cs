using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class ResetPasswordViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        public User User { get; set; }
        public string NewPassword { get; set; }
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
        public async void EditPassword()
        {
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            //Value = true;
            if (string.IsNullOrEmpty(NewPassword))
            {
                //await Application.Current.MainPage.DisplayAlert(Languages.Error, Languages.UsernameValidation, Languages.Ok);
                Value = true;
                return;
            }
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/user/resetUserPwd?userId=590&password=YWFhYWFh";//+ NewPassword;
            Debug.WriteLine("********url*************");
            Debug.WriteLine(url);
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));

            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Put, url);
            //req.RequestUri = new Uri(url);
            //req.Method = HttpMethod.Put;
            req.Headers.Add("Accept", "application/json");
            HttpResponseMessage response = await client.SendAsync(req);
            Debug.WriteLine("********response*************");
            Debug.WriteLine(response);
            //var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                return;
            }
            var result = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("********result*************");
            Debug.WriteLine(result);
            //Value = false;
            //MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Password Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand ResetPassword 
        {
            get
            {
                return new Command(() =>
                {
                     EditPassword();
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
