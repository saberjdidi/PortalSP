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
    public class SiapecViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Siapec> _siapec;
        private bool isRefreshing;
        private string filter;
        private List<Siapec> siapecList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Siapec> SIAPEC
        {
            get { return _siapec; }
            set
            {
                _siapec = value;
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
        public SiapecViewModel()
        {
            apiService = new ApiServices();
            GetSIAPEC();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetSIAPEC();
            });
        }
        #endregion

        #region Sigleton
        static SiapecViewModel instance;
        public static SiapecViewModel GetInstance()
        {
            if (instance == null)
            {
                return new SiapecViewModel();
            }

            return instance;
        }

        public void Update(Siapec siapec)
        {
            IsRefreshing = true;
            var oldSiapec = siapecList
                .Where(p => p.id == siapec.id)
                .FirstOrDefault();
            oldSiapec = siapec;
            SIAPEC = new ObservableCollection<Siapec>(siapecList);
            IsRefreshing = false;
        }
        public async Task Delete(Siapec siapec)
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
            var response = await apiService.Delete<Siapec>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/siapec/delete/" + siapec.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            siapecList.Remove(siapec);
            SIAPEC = new ObservableCollection<Siapec>(siapecList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetSIAPEC()
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
            var timestamp = DateTime.Now.ToFileTime();
            var response = await apiService.GetListWithCoockie<Siapec>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/siapec/list?sortedBy=description&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            siapecList = (List<Siapec>)response.Result;
            SIAPEC = new ObservableCollection<Siapec>(siapecList);
            IsRefreshing = false;
            if (SIAPEC.Count() == 0)
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
                return new RelayCommand(GetSIAPEC);
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
                SIAPEC = new ObservableCollection<Siapec>(siapecList);
            }
            else
            {
                SIAPEC = new ObservableCollection<Siapec>(
                    siapecList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.description.ToLower().Contains(Filter.ToLower())));
            }
            if (SIAPEC.Count() == 0)
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
