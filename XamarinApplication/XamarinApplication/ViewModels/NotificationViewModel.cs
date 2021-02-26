using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Services;
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
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
   public class NotificationViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Notification> _notification;
        private bool isRefreshing;
        private List<Notification> notificationList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Notification> Notifications
        {
            get { return _notification; }
            set
            {
                _notification = value;
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
        public NotificationViewModel()
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
        static NotificationViewModel instance;
        public static NotificationViewModel GetInstance()
        {
            if (instance == null)
            {
                return new NotificationViewModel();
            }

            return instance;
        }

        public void Update(Notification notification)
        {
            IsRefreshing = true;
            var oldNotification = notificationList
                .Where(p => p.id == notification.id)
                .FirstOrDefault();
            oldNotification = notification;
            Notifications = new ObservableCollection<Notification>(notificationList);
            IsRefreshing = false;
        }
        public async Task Delete(Notification notification)
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
            var response = await apiService.Delete<Notification>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/notification/delete/" + notification.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            notificationList.Remove(notification);
            Notifications = new ObservableCollection<Notification>(notificationList);

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
            var response = await apiService.GetListWithCoockie<Notification>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/notification/list?sortedBy=message&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            notificationList = (List<Notification>)response.Result;
            Notifications = new ObservableCollection<Notification>(notificationList);
            IsRefreshing = false;
            if (Notifications.Count() == 0)
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
        public ICommand NewNotification
        {
            get
            {
                return new Command(() =>
                {
                    if (Notifications.Count() == 0)
                    {
                        IsVisibleStatus = true;
                        PopupNavigation.Instance.PushAsync(new NewNotificationPage());
                    }
                    else
                    {
                        IsVisibleStatus = false;
                    }
                    
                });
            }
        }
        #endregion
    }
}
