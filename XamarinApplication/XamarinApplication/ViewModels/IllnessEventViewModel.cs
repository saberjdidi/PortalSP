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
   public class IllnessEventViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<IllnessEvent> _illnessEvent;
        private bool isRefreshing;
        private string filter;
        private List<IllnessEvent> illnessEventList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<IllnessEvent> IllnessEvents
        {
            get { return _illnessEvent; }
            set
            {
                _illnessEvent = value;
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
        public IllnessEventViewModel()
        {
            apiService = new ApiServices();
            GetIllnessEvent();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetIllnessEvent();
            });
        }
        #endregion

        #region Sigleton
        static IllnessEventViewModel instance;
        public static IllnessEventViewModel GetInstance()
        {
            if (instance == null)
            {
                return new IllnessEventViewModel();
            }

            return instance;
        }

        public void Update(IllnessEvent illnessEvent)
        {
            IsRefreshing = true;
            var oldIllnessEvent = illnessEventList
                .Where(p => p.id == illnessEvent.id)
                .FirstOrDefault();
            oldIllnessEvent = illnessEvent;
            IllnessEvents = new ObservableCollection<IllnessEvent>(illnessEventList);
            IsRefreshing = false;
        }
        public async Task Delete(IllnessEvent illnessEvent)
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
            var response = await apiService.Delete<IllnessEvent>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/illnessEvent/delete/" + illnessEvent.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            illnessEventList.Remove(illnessEvent);
            IllnessEvents = new ObservableCollection<IllnessEvent>(illnessEventList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetIllnessEvent()
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
            var response = await apiService.GetListWithCoockie<IllnessEvent>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/illnessEvent/list?sortedBy=code&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            illnessEventList = (List<IllnessEvent>)response.Result;
            IllnessEvents = new ObservableCollection<IllnessEvent>(illnessEventList);
            IsRefreshing = false;
            if (IllnessEvents.Count() == 0)
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
                return new RelayCommand(GetIllnessEvent);
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
                IllnessEvents = new ObservableCollection<IllnessEvent>(illnessEventList);
            }
            else
            {
                IllnessEvents = new ObservableCollection<IllnessEvent>(
                    illnessEventList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.description.ToLower().Contains(Filter.ToLower())));
            }

            if (IllnessEvents.Count() == 0)
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
