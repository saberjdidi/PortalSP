using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
   public class RequestHistoricRoleViewModel : BaseViewModel
    {
        #region Attributes
        private bool isRefreshing;
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public RequestHistoricRoleViewModel()
        {
            GetRequest();
        }
        #endregion

        #region Propertie
        public int IdRequest { get; set; }
        public Attachment AttachmentId { get; set; }
        private Request _request;
        public Request Request
        {
            get { return _request; }
            set
            {
                _request = value;
                OnPropertyChanged();
            }
        }
        public bool IsRefreshing
        {
            get
            {
                return isRefreshing;
            }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void GetRequest()
        {

            MessagingCenter.Subscribe<PassIdPatient>(this, "RequestId", async (value) =>
            {
                IdRequest = value.idPatient;
                Debug.WriteLine("********Id of request*************");
                Debug.WriteLine(IdRequest);
                IsRefreshing = true;
                var timestamp = DateTime.Now.ToFileTime();
                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/request/getById?id=" + IdRequest + "&time=" + timestamp;
                Debug.WriteLine("********url*************");
                Debug.WriteLine(url);
                client.BaseAddress = new Uri(url);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    IsRefreshing = false;
                    await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                    return;
                }
                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                IsRefreshing = false;
                var list = JsonConvert.DeserializeObject<Request>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********list*************");
                Debug.WriteLine(list);
                Request = (Request)list;
            });
        }
        #endregion
    }
}
