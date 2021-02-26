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
    public class ClosureCalendarViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<ClosureCalendar> _closureCalendar;
        private bool isRefreshing;
        private string filter;
        private List<ClosureCalendar> closureCalendarList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<ClosureCalendar> ClosureCalendar
        {
            get { return _closureCalendar; }
            set
            {
                _closureCalendar = value;
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
        public ClosureCalendarViewModel()
        {
            apiService = new ApiServices();
            GetClosureCalendar();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetClosureCalendar();
            });
        }
        #endregion

        #region Sigleton
        static ClosureCalendarViewModel instance;
        public static ClosureCalendarViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ClosureCalendarViewModel();
            }

            return instance;
        }

        public void Update(ClosureCalendar closureCalendar)
        {
            IsRefreshing = true;
            var oldclosureCalendar = closureCalendarList
                .Where(p => p.id == closureCalendar.id)
                .FirstOrDefault();
            oldclosureCalendar = closureCalendar;
            ClosureCalendar = new ObservableCollection<ClosureCalendar>(closureCalendarList);
            IsRefreshing = false;
        }
        public async Task Delete(ClosureCalendar closureCalendar)
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
            var response = await apiService.Delete<ClosureCalendar>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/closurePeriod/delete/" + closureCalendar.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            closureCalendarList.Remove(closureCalendar);
            ClosureCalendar = new ObservableCollection<ClosureCalendar>(closureCalendarList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetClosureCalendar()
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
            var response = await apiService.GetListWithCoockie<ClosureCalendar>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/closurePeriod/list?sortedBy=code&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            closureCalendarList = (List<ClosureCalendar>)response.Result;
            ClosureCalendar = new ObservableCollection<ClosureCalendar>(closureCalendarList);
            IsRefreshing = false;
            if (ClosureCalendar.Count() == 0)
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
                return new RelayCommand(GetClosureCalendar);
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
                ClosureCalendar = new ObservableCollection<ClosureCalendar>(closureCalendarList);
            }
            else
            {
                ClosureCalendar = new ObservableCollection<ClosureCalendar>(
                    closureCalendarList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.period.ToLower().Contains(Filter.ToLower())));
            }
            if (ClosureCalendar.Count() == 0)
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
