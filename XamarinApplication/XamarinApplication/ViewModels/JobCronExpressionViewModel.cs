using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
   public class JobCronExpressionViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private JobCron _jobCron;
        private bool isRefreshing = false;
        bool _isVisibleStatus;
        private bool _showHide = false;
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
        public JobCronExpressionViewModel()
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
        static JobCronExpressionViewModel instance;
        public static JobCronExpressionViewModel GetInstance()
        {
            if (instance == null)
            {
                return new JobCronExpressionViewModel();
            }

            return instance;
        }

        /* public void Update(Icdo icdo)
         {
             IsRefreshing = true;
             var oldIcdo = icdoList
                 .Where(p => p.id == icdo.id)
                 .FirstOrDefault();
             oldIcdo = icdo;
             ICDO = new ObservableCollection<Icdo>(icdoList);
             IsRefreshing = false;
         }*/
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
            var response = await apiService.Delete<Configs>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/mailSender/deleteCron/"+ configs.id + "?id=@id",
                res);

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
            var response = await apiService.Delete<Addresses>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/mailSender/deleteAddress/" + address.id + "?id=@id",
                res);

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
            /* var response = await apiService.GetAttachmentWithCoockie<JobCron>(
                  "https://portalesp.smart-path.it",
                  "/Portalesp",
                  "/mailSender/list?sortedBy=addressMail&order=asc&time=" + timestamp,
                  res);
             if (!response.IsSuccess)
             {
                 IsRefreshing = false;
                 await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                 return;
             }
            JobCron = (JobCron)response.Result;*/
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/mailSender/list?sortedBy=addressMail&order=asc&time=" + timestamp;
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
            // var cron = new Quartz.CronExpression("0 50 9 ? * FRI,WED,TUE,THU,MON *");
            // DateTimeOffset? nextFire = cron.GetNextValidTimeAfter(DateTime.Now);
            //CronExpressionDescriptor.ExpressionDescriptor.GetDescription("0 0 14 ? * SAT *");
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
