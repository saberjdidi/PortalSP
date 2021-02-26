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
    public class ServiceViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Ambulatory> _ambulatory;
        private bool isRefreshing;
        private string filter;
        private List<Ambulatory> ambulatoryList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Ambulatory> Ambulatoires
        {
            get { return _ambulatory; }
            set
            {
                _ambulatory = value;
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
        public ServiceViewModel()
        {
            apiService = new ApiServices();
            GetService();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetService();
            });
        }
        #endregion

        #region Sigleton
        static ServiceViewModel instance;
        public static ServiceViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ServiceViewModel();
            }

            return instance;
        }

        public void Update(Ambulatory ambulatory)
        {
            IsRefreshing = true;
            var oldambulatory = ambulatoryList
                .Where(p => p.id == ambulatory.id)
                .FirstOrDefault();
            oldambulatory = ambulatory;
            Ambulatoires = new ObservableCollection<Ambulatory>(ambulatoryList);
            IsRefreshing = false;
        }
        public async Task Delete(Ambulatory ambulatory)
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
            var response = await apiService.Delete<Ambulatory>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/ambulatory/delete/" + ambulatory.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            ambulatoryList.Remove(ambulatory);
            Ambulatoires = new ObservableCollection<Ambulatory>(ambulatoryList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetService()
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
            var response = await apiService.GetListWithCoockie<Ambulatory>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/ambulatory/list?sortedBy=code&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            ambulatoryList = (List<Ambulatory>)response.Result;
            Ambulatoires = new ObservableCollection<Ambulatory>(ambulatoryList);
            IsRefreshing = false;
            if (Ambulatoires.Count() == 0)
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
                return new RelayCommand(GetService);
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
                Ambulatoires = new ObservableCollection<Ambulatory>(ambulatoryList);
            }
            else
            {
                Ambulatoires = new ObservableCollection<Ambulatory>(
                    ambulatoryList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.description.ToLower().Contains(Filter.ToLower())));
            }
            if (Ambulatoires.Count() == 0)
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
