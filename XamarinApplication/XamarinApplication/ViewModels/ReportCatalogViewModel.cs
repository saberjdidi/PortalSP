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
   public class ReportCatalogViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<ReportCatalog> _reportCatalog;
        private bool isRefreshing;
        private string filter;
        private List<ReportCatalog> reportCatalogList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<ReportCatalog> ReportCatalogs
        {
            get { return _reportCatalog; }
            set
            {
                _reportCatalog = value;
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
        public ReportCatalogViewModel()
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
        static ReportCatalogViewModel instance;
        public static ReportCatalogViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ReportCatalogViewModel();
            }

            return instance;
        }

        public void Update(ReportCatalog reportCatalog)
        {
            IsRefreshing = true;
            var oldreportCatalog = reportCatalogList
                .Where(p => p.id == reportCatalog.id)
                .FirstOrDefault();
            oldreportCatalog = reportCatalog;
            ReportCatalogs = new ObservableCollection<ReportCatalog>(reportCatalogList);
            IsRefreshing = false;
        }
        public async Task Delete(ReportCatalog reportCatalog)
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
            var response = await apiService.Delete<ReportCatalog>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/reportCatalog/delete/" + reportCatalog.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            reportCatalogList.Remove(reportCatalog);
            ReportCatalogs = new ObservableCollection<ReportCatalog>(reportCatalogList);

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
            var response = await apiService.GetListWithCoockie<ReportCatalog>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/reportCatalog/list?sortedBy=description&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            reportCatalogList = (List<ReportCatalog>)response.Result;
            ReportCatalogs = new ObservableCollection<ReportCatalog>(reportCatalogList);
            IsRefreshing = false;
            if (ReportCatalogs.Count() == 0)
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
                ReportCatalogs = new ObservableCollection<ReportCatalog>(reportCatalogList);
            }
            else
            {
                ReportCatalogs = new ObservableCollection<ReportCatalog>(
                    reportCatalogList.Where(
                        l => l.icdo.description.ToLower().Contains(Filter.ToLower()) ||
                        l.ragService.code.ToLower().Contains(Filter.ToLower())));
            }
            if (ReportCatalogs.Count() == 0)
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
