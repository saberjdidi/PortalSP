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
   public class TaxRegimeViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<TaxRegime> _taxRegime;
        private bool isRefreshing;
        private string filter;
        private List<TaxRegime> taxRegimeList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<TaxRegime> TaxRegimes
        {
            get { return _taxRegime; }
            set
            {
                _taxRegime = value;
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
        public TaxRegimeViewModel()
        {
            apiService = new ApiServices();
            GetTaxRegime();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetTaxRegime();
            });
        }
        #endregion

        #region Sigleton
        static TaxRegimeViewModel instance;
        public static TaxRegimeViewModel GetInstance()
        {
            if (instance == null)
            {
                return new TaxRegimeViewModel();
            }

            return instance;
        }

        public void Update(TaxRegime taxRegime)
        {
            IsRefreshing = true;
            var oldTaxRegime = taxRegimeList
                .Where(p => p.id == taxRegime.id)
                .FirstOrDefault();
            oldTaxRegime = taxRegime;
            TaxRegimes = new ObservableCollection<TaxRegime>(taxRegimeList);
            IsRefreshing = false;
        }
        public async Task Delete(TaxRegime taxRegime)
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
            var response = await apiService.Delete<TaxRegime>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/taxRegime/delete/" + taxRegime.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            taxRegimeList.Remove(taxRegime);
            TaxRegimes = new ObservableCollection<TaxRegime>(taxRegimeList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetTaxRegime()
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
            var response = await apiService.GetListWithCoockie<TaxRegime>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/taxRegime/list?sortedBy=description&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            taxRegimeList = (List<TaxRegime>)response.Result;
            TaxRegimes = new ObservableCollection<TaxRegime>(taxRegimeList);
            IsRefreshing = false;
            if (TaxRegimes.Count() == 0)
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
                return new RelayCommand(GetTaxRegime);
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
                TaxRegimes = new ObservableCollection<TaxRegime>(taxRegimeList);
            }
            else
            {
                TaxRegimes = new ObservableCollection<TaxRegime>(
                    taxRegimeList.Where(
                        l => l.code.ToLower().StartsWith(Filter.ToLower()) ||
                        l.description.ToLower().StartsWith(Filter.ToLower())));
            }
            if (TaxRegimes.Count() == 0)
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
