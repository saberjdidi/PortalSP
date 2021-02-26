using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class ConfigurationPostedReportViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private JobCron _jobCron;
        private bool isRefreshing = false;
        bool _isVisibleStatus;
        #endregion

        #region Properties
        public JobCron JobCron
        {
            get { return _jobCron; }
            set
            {
                _jobCron = value;
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
        public bool IsVisibleStatus
        {
            get { return _isVisibleStatus; }
            set
            {
                _isVisibleStatus = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public ConfigurationPostedReportViewModel()
        {
            apiService = new ApiServices();
            GetJobCron();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetJobCron();
            });
        }
        #endregion

        #region Sigleton
        static ConfigurationPostedReportViewModel instance;
        public static ConfigurationPostedReportViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ConfigurationPostedReportViewModel();
            }

            return instance;
        }
        public async Task DeleteJobCron(Configs configs)
        {
            IsRefreshing = true;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var _config = new Configs
            {
                id = configs.id,
                cron = configs.cron,
                code = configs.code
            };
            var response = await apiService.Save<Configs>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/postedReports/deleteMail",
                res,
                _config);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            IsRefreshing = false;
        }
        public async Task DeleteAddressEmail(Addresses address)
        {
            IsRefreshing = true;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var _address = new Addresses
            {
                id = address.id,
                addressMail = address.addressMail,
                code = address.code
            };
            var response = await apiService.Save<Addresses>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/postedReports/deleteMail",
                res,
                _address);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetJobCron()
        {
            IsRefreshing = true;
            var connection = await apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                IsRefreshing = false;
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
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/postedReports/listMails";
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
            var list = JsonConvert.DeserializeObject<JobCron>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Debug.WriteLine("********list*************");
            Debug.WriteLine(list);
            JobCron = (JobCron)list;
            IsRefreshing = false;
           /* if (JobCron.configs.Count() == 0)
            {
                IsVisibleStatus = true;
            }
            else if (JobCron.addresses.Count() == 0)
            {
                IsVisibleStatus = true;
            }
            else
            {
                IsVisibleStatus = false;
            }*/
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetJobCron);
            }
        }

        #endregion
    }
}
