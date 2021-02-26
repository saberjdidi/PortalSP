using GalaSoft.MvvmLight.Command;
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
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class RequestCheckListViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        public Request Request { get; set; }
        private ObservableCollection<CheckList> _checkList;
        private List<CheckList> checkList;
        bool _isVisibleStatus;
        private bool isRefreshing;
        #endregion

        #region Properties
        public ObservableCollection<CheckList> CheckList
        {
            get { return _checkList; }
            set
            {
                _checkList = value;
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

        #region Constructors
        public RequestCheckListViewModel()
        {
            apiService = new ApiServices();
            GetCheckList();
        }
        #endregion

        #region Methods
        public async void GetCheckList()
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
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<CheckList>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/diagnosis/getActiveCheckList?criteria=",
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            checkList = (List<CheckList>)response.Result;
            CheckList = new ObservableCollection<CheckList>(checkList);
            IsRefreshing = false;
            if (CheckList.Count() == 0)
            {
                IsVisibleStatus = true;
            }
            else
            {
                IsVisibleStatus = false;
            }
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetCheckList);
            }
        }
        public ICommand PreliminaryReport
        {
            get
            {
                return new Command(async () =>
                {
                    var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
                    var cookie = Settings.Cookie;
                    var res = cookie.Substring(11, 32);
                    IsRefreshing = true;
                    var _report = new PreliminaryReport
                    {
                        fromCheckList = true,
                        idReq = Request.id
                    };

                    var requestJson = JsonConvert.SerializeObject(_report);
                    Debug.WriteLine("********request*************");
                    Debug.WriteLine(requestJson);
                    var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

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
                    IsRefreshing = false;
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

                        await DependencyService.Get<ISave>().SaveAndView(Request.code + "-" + dateNow + ".pdf", "application/pdf", stream);
                    }
                });
            }
        }
        #endregion
    }
}
