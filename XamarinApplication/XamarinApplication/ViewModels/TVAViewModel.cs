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
   public class TVAViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<TVA> _tva;
        private bool isRefreshing;
        private string filter;
        private List<TVA> tvaList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<TVA> TVA
        {
            get { return _tva; }
            set
            {
                _tva = value;
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
        public TVAViewModel()
        {
            apiService = new ApiServices();
            GetTVA();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetTVA();
            });
        }
        #endregion

        #region Sigleton
        static TVAViewModel instance;
        public static TVAViewModel GetInstance()
        {
            if (instance == null)
            {
                return new TVAViewModel();
            }

            return instance;
        }

        public void Update(TVA tva)
        {
            IsRefreshing = true;
            var oldtva = tvaList
                .Where(p => p.id == tva.id)
                .FirstOrDefault();
            oldtva = tva;
            TVA = new ObservableCollection<TVA>(tvaList);
            IsRefreshing = false;
        }
        public async Task Delete(TVA tva)
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
            var response = await apiService.Delete<TVA>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/tvaCode/delete/" + tva.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            tvaList.Remove(tva);
            TVA = new ObservableCollection<TVA>(tvaList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetTVA()
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
            var response = await apiService.GetListWithCoockie<TVA>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/tvaCode/list?sortedBy=description&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            tvaList = (List<TVA>)response.Result;
            TVA = new ObservableCollection<TVA>(tvaList);
            IsRefreshing = false;
            if (TVA.Count() == 0)
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
                return new RelayCommand(GetTVA);
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
                TVA = new ObservableCollection<TVA>(tvaList);
            }
            else
            {
                TVA = new ObservableCollection<TVA>(
                    tvaList.Where(
                        l => l.code.ToLower().StartsWith(Filter.ToLower()) ||
                        l.description.ToLower().StartsWith(Filter.ToLower())));

            }
            if (TVA.Count() == 0)
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
