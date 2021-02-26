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
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class ArchiveRequestViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        public Request Request { get; set; }
        private ObservableCollection<RequestArchive> _archive;
        private List<RequestArchive> archiveList;
        bool _isVisibleStatus;
        private bool isRefreshing;
        #endregion

        #region Properties
        public ObservableCollection<RequestArchive> RequestArchives
        {
            get { return _archive; }
            set
            {
                _archive = value;
                OnPropertyChanged();
            }
        }
        public List<string> BiologicalMaterials;
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
        public ArchiveRequestViewModel()
        {
            apiService = new ApiServices();
            GetList();
        }
        #endregion

        #region Methods
        public async void GetList()
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
            var response = await apiService.GetListWithCoockie<string>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/biologicalMaterial/getBarCodeByRequest?requestId="+ Request.id,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            BiologicalMaterials = (List<string>)response.Result;
            Debug.WriteLine("********BiologicalMaterials*************");
            Debug.WriteLine(BiologicalMaterials);
            //getArchive
            var barCode = new BarCodeBiologicalMaterials
            {
               barCodeBiologicalMaterials = BiologicalMaterials
            };

            var requestJson = JsonConvert.SerializeObject(barCode);
            Debug.WriteLine("********request*************");
            Debug.WriteLine(requestJson);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/request/getArchive";
            Debug.WriteLine("********url*************");
            Debug.WriteLine(url);
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var response2 = await client.PostAsync(url, content);
            if (!response2.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response2.StatusCode.ToString(), "ok");
                return;
            }
            var result = await response2.Content.ReadAsStringAsync();
            Debug.WriteLine("********result*************");
            Debug.WriteLine(result);
            var list = JsonConvert.DeserializeObject<List<RequestArchive>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            archiveList = (List<RequestArchive>)list;
            RequestArchives = new ObservableCollection<RequestArchive>(archiveList);
             IsRefreshing = false;
             if (RequestArchives.Count() == 0)
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
                return new RelayCommand(GetList);
            }
        }
        #endregion
    }
    #region Model
    public class BarCodeBiologicalMaterials
    {
        public List<string> barCodeBiologicalMaterials { get; set; }
    }
    #endregion
}
