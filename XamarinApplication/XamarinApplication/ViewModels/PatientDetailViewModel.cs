using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
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
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
    public class PatientDetailViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        public Patient PatientId { get; set; }
        private Patient _patient;
        public Patient Patient
        {
            get { return _patient; }
            set
            {
                _patient = value;
                OnPropertyChanged();
            }
        }
        public int IdPatient { get; set; }
        #endregion

        #region Constructors
        public PatientDetailViewModel()
        {
            //Navigation = _navigation;
            GetPatient();
        }
        #endregion

        #region Methods
        public async void GetPatient()
        {
            /*  var connection = await apiService.CheckConnection();

              if (!connection.IsSuccess)
              {
                  await Application.Current.MainPage.DisplayAlert(
                      "Error",
                      connection.Message,
                      "Ok");
                  await Application.Current.MainPage.Navigation.PopAsync();
                  return;
              }

              var timestamp = DateTime.Now.ToFileTime();
              var cookie = Settings.Cookie;  //.Split(11, 33)
              var res = cookie.Substring(11, 32);
              MessagingCenter.Subscribe<PassIdPatient>(this, "PatientId", async (value) =>
              {
                  IdPatient = value.idPatient;
                  Debug.WriteLine("********Id of patient*************");
                  Debug.WriteLine(IdPatient);

                  
              });*/

            MessagingCenter.Subscribe<PassIdPatient>(this, "PatientId", async(value) =>
            {
                IdPatient = value.idPatient;
                Debug.WriteLine("********Id of patient*************");
                Debug.WriteLine(IdPatient);

            var timestamp = DateTime.Now.ToFileTime();
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/patient/getById?id="+ IdPatient + "&time="+ timestamp;
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
            var list = JsonConvert.DeserializeObject<Patient>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Patient = (Patient)list;
            });
        }
        #endregion
        #region Commands
        public ICommand NewRequest
        {
            get
            {
                return new Command(() =>
                {
                    PopupNavigation.Instance.PushAsync(new NewRequestPage(Patient));
                });
            }
        }
        #endregion
    }
}
