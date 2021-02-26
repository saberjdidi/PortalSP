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
    public class RequestCatalogViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Requestcatalog> _requestCatalog;
        private bool isRefreshing;
        private string filter;
        private List<Requestcatalog> requestCatalogList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Requestcatalog> RequestCatalog
        {
            get { return _requestCatalog; }
            set
            {
                _requestCatalog = value;
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
        public RequestCatalogViewModel()
        {
            apiService = new ApiServices();
            GetRequestCatalog();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetRequestCatalog();
            });
        }
        #endregion

        #region Sigleton
        static RequestCatalogViewModel instance;
        public static RequestCatalogViewModel GetInstance()
        {
            if (instance == null)
            {
                return new RequestCatalogViewModel();
            }

            return instance;
        }

        public void Update(Requestcatalog requestcatalog)
        {
            IsRefreshing = true;
            var oldrequestcatalog = requestCatalogList
                .Where(p => p.id == requestcatalog.id)
                .FirstOrDefault();
            oldrequestcatalog = requestcatalog;
            RequestCatalog = new ObservableCollection<Requestcatalog>(requestCatalogList);
            IsRefreshing = false;
        }
        public async Task Delete(Requestcatalog requestcatalog)
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
            var response = await apiService.Delete<Requestcatalog>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/requestCatalog/delete/" + requestcatalog.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            requestCatalogList.Remove(requestcatalog);
            RequestCatalog = new ObservableCollection<Requestcatalog>(requestCatalogList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetRequestCatalog()
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
            var response = await apiService.GetListWithCoockie<Requestcatalog>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/requestCatalog/list?sortedBy=code&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            requestCatalogList = (List<Requestcatalog>)response.Result;
            RequestCatalog = new ObservableCollection<Requestcatalog>(requestCatalogList);
            IsRefreshing = false;
            if (RequestCatalog.Count() == 0)
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
                return new RelayCommand(GetRequestCatalog);
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
                RequestCatalog = new ObservableCollection<Requestcatalog>(requestCatalogList);
            }
            else
            {
                RequestCatalog = new ObservableCollection<Requestcatalog>(
                    requestCatalogList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.description.ToLower().Contains(Filter.ToLower())));
            }
            if (RequestCatalog.Count() == 0)
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
