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
   public class NazLocalViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<NazLocal> _nazLocal;
        private bool isRefreshing;
        private string filter;
        private List<NazLocal> nazLocalList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<NazLocal> NazLocal
        {
            get { return _nazLocal; }
            set
            {
                _nazLocal = value;
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
        public NazLocalViewModel()
        {
            apiService = new ApiServices();
            GetNazLocal();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetNazLocal();
            });
        }
        #endregion

        #region Sigleton
        static NazLocalViewModel instance;
        public static NazLocalViewModel GetInstance()
        {
            if (instance == null)
            {
                return new NazLocalViewModel();
            }

            return instance;
        }

        public void Update(NazLocal nazLocal)
        {
            IsRefreshing = true;
            var oldnazLocal = nazLocalList
                .Where(p => p.id == nazLocal.id)
                .FirstOrDefault();
            oldnazLocal = nazLocal;
            NazLocal = new ObservableCollection<NazLocal>(nazLocalList);
            IsRefreshing = false;
        }
        public async Task Delete(NazLocal nazLocal)
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
            var response = await apiService.Delete<NazLocal>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/nazLocal/delete/" + nazLocal.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            nazLocalList.Remove(nazLocal);
            NazLocal = new ObservableCollection<NazLocal>(nazLocalList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetNazLocal()
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
            var response = await apiService.GetListWithCoockie<NazLocal>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/nazLocal/list?sortedBy=code&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            nazLocalList = (List<NazLocal>)response.Result;
            NazLocal = new ObservableCollection<NazLocal>(nazLocalList);
            IsRefreshing = false;
            if (NazLocal.Count() == 0)
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
                return new RelayCommand(GetNazLocal);
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
                NazLocal = new ObservableCollection<NazLocal>(nazLocalList);
            }
            else
            {
                NazLocal = new ObservableCollection<NazLocal>(
                    nazLocalList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.description.ToLower().Contains(Filter.ToLower())));
            }
            if (NazLocal.Count() == 0)
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
