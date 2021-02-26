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
   public class InstrumentViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Instrument> _instrument;
        private List<Instrument> instrumentList;
        private bool isRefreshing;
        private string filter;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Instrument> Instruments
        {
            get { return _instrument; }
            set
            {
                _instrument = value;
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
        public InstrumentViewModel()
        {
            apiService = new ApiServices();
            GetInstrument();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetInstrument();
            });
        }
        #endregion

        #region Sigleton
        static InstrumentViewModel instance;
        public static InstrumentViewModel GetInstance()
        {
            if (instance == null)
            {
                return new InstrumentViewModel();
            }

            return instance;
        }

        public void Update(Instrument instrument)
        {
            IsRefreshing = true;
            var oldInstrument = instrumentList
                .Where(p => p.id == instrument.id)
                .FirstOrDefault();
            oldInstrument = instrument;
            Instruments = new ObservableCollection<Instrument>(instrumentList);
            IsRefreshing = false;
        }
        public async Task Activate(Instrument instrument)
        {
            IsRefreshing = true;
            var oldInstrument = instrumentList
                .Where(p => p.id == instrument.id)
                .FirstOrDefault();
            oldInstrument = instrument;
            Instruments = new ObservableCollection<Instrument>(instrumentList);
            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetInstrument()
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
            var response = await apiService.GetListWithCoockie<Instrument>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/instrument/list?sortedBy=name&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            instrumentList = (List<Instrument>)response.Result;
            Instruments = new ObservableCollection<Instrument>(instrumentList);
            IsRefreshing = false;
            if (Instruments.Count() == 0)
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
                return new RelayCommand(GetInstrument);
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
                Instruments = new ObservableCollection<Instrument>(instrumentList);
            }
            else
            {
                Instruments = new ObservableCollection<Instrument>(
                    instrumentList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.name.ToLower().Contains(Filter.ToLower()) ||
                        l.description.ToLower().Contains(Filter.ToLower())));
            }

            if (Instruments.Count() == 0)
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
