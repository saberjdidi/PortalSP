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
   public class RequestCompilationViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<RequestCompilation> _requestCompilation;
        private bool isRefreshing;
        private string filter;
        private List<RequestCompilation> requestCompilationList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<RequestCompilation> RequestCompilations
        {
            get { return _requestCompilation; }
            set
            {
                _requestCompilation = value;
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
        public RequestCompilationViewModel()
        {
            apiService = new ApiServices();
            GetList();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetList();
            });
        }
        #endregion

        #region Sigleton
        static RequestCompilationViewModel instance;
        public static RequestCompilationViewModel GetInstance()
        {
            if (instance == null)
            {
                return new RequestCompilationViewModel();
            }

            return instance;
        }

        public void Update(RequestCompilation requestCompilation)
        {
            IsRefreshing = true;
            var oldrequestCompilation = requestCompilationList
                .Where(p => p.id == requestCompilation.id)
                .FirstOrDefault();
            oldrequestCompilation = requestCompilation;
            RequestCompilations = new ObservableCollection<RequestCompilation>(requestCompilationList);
            IsRefreshing = false;
        }
        public async Task Delete(RequestCompilation requestCompilation)
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
            var response = await apiService.Delete<RequestCompilation>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/reportGenerator/delete/" + requestCompilation.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            requestCompilationList.Remove(requestCompilation);
            RequestCompilations = new ObservableCollection<RequestCompilation>(requestCompilationList);

            IsRefreshing = false;
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
            var timestamp = DateTime.Now.ToFileTime();
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<RequestCompilation>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/reportGenerator/listReportsOfClient?sortedBy=code&order1=asc&clientId=147&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            requestCompilationList = (List<RequestCompilation>)response.Result;
            RequestCompilations = new ObservableCollection<RequestCompilation>(requestCompilationList);
            IsRefreshing = false;
            if (RequestCompilations.Count() == 0)
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
                RequestCompilations = new ObservableCollection<RequestCompilation>(requestCompilationList);
            }
            else
            {
                RequestCompilations = new ObservableCollection<RequestCompilation>(
                    requestCompilationList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.reportName.ToLower().Contains(Filter.ToLower())));
            }
            if (RequestCompilations.Count() == 0)
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
