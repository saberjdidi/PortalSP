using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class RequestModelViewModel: BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<RagService> _requestModel;
        private bool isRefreshing;
        private string filter;
        private List<RagService> ragServiceList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<RagService> RequestModels
        {
            get { return _requestModel; }
            set
            {
                _requestModel = value;
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
        public string Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                OnPropertyChanged();
                Search();
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
        public bool ShowHide
        {
            get => _showHide;
            set
            {
                _showHide = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public RequestModelViewModel()
        {
            apiService = new ApiServices();
            GetRequestModel();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetRequestModel();
            });
        }
        #endregion

        #region Sigleton
        static RequestModelViewModel instance;
        public static RequestModelViewModel GetInstance()
        {
            if (instance == null)
            {
                return new RequestModelViewModel();
            }

            return instance;
        }

        public void Update(RagService requestcatalog)
        {
            IsRefreshing = true;
            var oldrequestcatalog = ragServiceList
                .Where(p => p.id == requestcatalog.id)
                .FirstOrDefault();
            oldrequestcatalog = requestcatalog;
            RequestModels = new ObservableCollection<RagService>(ragServiceList);
            IsRefreshing = false;
        }
        public async Task Delete(RagService requestcatalog)
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
            var response = await apiService.Delete<RagService>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/ragService/delete/" + requestcatalog.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            ragServiceList.Remove(requestcatalog);
            RequestModels = new ObservableCollection<RagService>(ragServiceList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetRequestModel()
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
            var response = await apiService.GetListWithCoockie<RagService>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/ragService/list?sortedBy=description&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            ragServiceList = (List<RagService>)response.Result;
            RequestModels = new ObservableCollection<RagService>(ragServiceList);
            IsRefreshing = false;
            if (RequestModels.Count() == 0)
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
                return new RelayCommand(GetRequestModel);
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(Search);
            }
        }

        private void Search()
        {
            if (string.IsNullOrEmpty(Filter))
            {
                RequestModels = new ObservableCollection<RagService>(ragServiceList);
            }
            else
            {
                RequestModels = new ObservableCollection<RagService>(
                    ragServiceList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.description.ToLower().Contains(Filter.ToLower())));
            }
            if (RequestModels.Count() == 0)
            {
                IsVisibleStatus = true;
            }
            else
            {
                IsVisibleStatus = false;
            }
        }
        public ICommand OpenSearchBar
        {
            get
            {
                return new Command(() =>
                {
                    if (ShowHide == false)
                    {
                        ShowHide = true;
                    }
                    else
                    {
                        ShowHide = false;
                    }
                });
            }
        }
        #endregion
    }
}
