using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
    public class RequestPatientViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Request> _request;
        private List<Request> requestList;
        private bool isVisible;
        #endregion

        #region Properties
        public Patient Patient { get; set; }
        public ObservableCollection<Request> Requests
        {
            get { return _request; }
            set
            {
                _request = value;
                OnPropertyChanged();
            }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion 

        #region Constructors
        public RequestPatientViewModel()
        {
            apiService = new ApiServices();
            GetRequests();
            instance = this;
        }
        #endregion

        #region Methods
        public async void GetRequests()
        {
            var connection = await apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }
            var _search = new SearchModel
            {
                id1 = Patient.id,
                maxResult = 50,
                order = "desc",
                sortedBy = "request_creation_date",
                status = "ALL"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            Debug.WriteLine("********cookie ViewModel*************");
            Debug.WriteLine(cookie);
            var response = await apiService.PostRequest<Request>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/request/searchRequest?mobile=mobile", ///request/searchByExample
                 res,
                 _search);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            requestList = (List<Request>)response.Result;
            Requests = new ObservableCollection<Request>(requestList);
            if (Requests.Count() == 0)
            {
                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }

        }
        #endregion

        #region Sigleton
        static RequestPatientViewModel instance;
        public static RequestPatientViewModel GetInstance()
        {
            if (instance == null)
            {
                return new RequestPatientViewModel();
            }

            return instance;
        }
        public async Task Download(Request requestPatient)
        {

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }
            var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var _report = new PreliminaryReport
            {
                fromCheckList = true,
                idReq = requestPatient.id //requestPatient.requests.Select(r => r.id).FirstOrDefault()
            };

            var request = JsonConvert.SerializeObject(_report);
            Debug.WriteLine("********request*************");
            Debug.WriteLine(request);
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/doctorAvis/printReportTrackerFromTemplate";
            Debug.WriteLine("********url*************");
            Debug.WriteLine(url);
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                return;
            }
            var result = await response.Content.ReadAsStreamAsync();
            Debug.WriteLine("********result*************");
            Debug.WriteLine(result);
            using (var streamReader = new MemoryStream())
            {
                result.CopyTo(streamReader);
                byte[] bytes = streamReader.ToArray();
                MemoryStream stream = new MemoryStream(bytes);
                Debug.WriteLine("********stream*************");
                Debug.WriteLine(stream);
                if (stream == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Warning", "Data is Empty", "ok");
                    return;
                }

                await DependencyService.Get<ISave>().SaveAndView(requestPatient.code+"-"+ dateNow + ".pdf", "application/pdf", stream);
                //await DependencyService.Get<ISave>().SaveAndView(requestPatient.requests.Select(r => r.code).FirstOrDefault()+"-"+ dateNow + ".pdf", "application/pdf", stream);
            }
        }
        #endregion
    }
    public class PreliminaryReport
    {
        public bool fromCheckList { get; set; }
        public int idReq { get; set; }
    }
}
