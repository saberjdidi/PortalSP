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
   public class ServiceDocumentViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<ServiceDocument> _serviceDocument;
        private bool isRefreshing;
        private string filter;
        private List<ServiceDocument> serviceDocumentList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<ServiceDocument> ServiceDocuments
        {
            get { return _serviceDocument; }
            set
            {
                _serviceDocument = value;
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
        public ServiceDocumentViewModel()
        {
            apiService = new ApiServices();
            GetServiceDocument();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetServiceDocument();
            });
        }
        #endregion

        #region Sigleton
        static ServiceDocumentViewModel instance;
        public static ServiceDocumentViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ServiceDocumentViewModel();
            }

            return instance;
        }

        public void Update(ServiceDocument serviceDocument)
        {
            IsRefreshing = true;
            var oldServiceDocument = serviceDocumentList
                .Where(p => p.id == serviceDocument.id)
                .FirstOrDefault();
            oldServiceDocument = serviceDocument;
            ServiceDocuments = new ObservableCollection<ServiceDocument>(serviceDocumentList);
            IsRefreshing = false;
        }
        public async Task Delete(ServiceDocument serviceDocument)
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
            var response = await apiService.Delete<ServiceDocument>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/serviceDocument/delete/" + serviceDocument.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            serviceDocumentList.Remove(serviceDocument);
            ServiceDocuments = new ObservableCollection<ServiceDocument>(serviceDocumentList);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetServiceDocument()
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
            var _searchModel = new SearchModel
            {
                order = "asc",
                sortedBy = "code"
            };
            var response = await apiService.PostRequest<ServiceDocument>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/serviceDocument/loadServiceDoc",
                res,
                _searchModel);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            serviceDocumentList = (List<ServiceDocument>)response.Result;
            ServiceDocuments = new ObservableCollection<ServiceDocument>(serviceDocumentList);
            IsRefreshing = false;
            if (ServiceDocuments.Count() == 0)
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
                return new RelayCommand(GetServiceDocument);
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
                ServiceDocuments = new ObservableCollection<ServiceDocument>(serviceDocumentList);
            }
            else
            {
                ServiceDocuments = new ObservableCollection<ServiceDocument>(
                    serviceDocumentList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.name.ToLower().Contains(Filter.ToLower())));
            }

            if (ServiceDocuments.Count() == 0)
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
