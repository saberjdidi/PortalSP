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
   public class RoomViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Room> _room;
        private List<Room> roomList;
        private bool isRefreshing;
        private string filter;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Room> Rooms
        {
            get { return _room; }
            set
            {
                _room = value;
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
        public RoomViewModel()
        {
            apiService = new ApiServices();
            GetRoom();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetRoom();
            });
        }
        #endregion

        #region Sigleton
        static RoomViewModel instance;
        public static RoomViewModel GetInstance()
        {
            if (instance == null)
            {
                return new RoomViewModel();
            }

            return instance;
        }

        public void Update(Room room)
        {
            IsRefreshing = true;
            var oldRoom = roomList
                .Where(p => p.id == room.id)
                .FirstOrDefault();
            oldRoom = room;
            Rooms = new ObservableCollection<Room>(roomList);
            IsRefreshing = false;
        }
        public async Task Activate(Room room)
        {
            IsRefreshing = true;
            var oldRoom = roomList
                .Where(p => p.id == room.id)
                .FirstOrDefault();
            oldRoom = room;
            Rooms = new ObservableCollection<Room>(roomList);
            IsRefreshing = false;
            /* IsRefreshing = true;

             var connection = await apiService.CheckConnection();
             if (!connection.IsSuccess)
             {
                 IsRefreshing = false;
                 await dialogService.ShowMessage("Error", connection.Message);
                 return;
             }
             var cookie = Settings.Cookie;  //.Split(11, 33)
             var res = cookie.Substring(11, 32);
             var response = await apiService.GetListWithCoockie<Room>(
                  "https://portalesp.smart-path.it",
                  "/Portalesp",
                  "/room/activate?id=" + room.id,
                  res);

             if (!response.IsSuccess)
             {
                 IsRefreshing = false;
                 await dialogService.ShowMessage(
                     "Error",
                     response.Message);
                 return;
             }

             roomList = (List<Room>)response.Result;
             Rooms = new ObservableCollection<Room>(roomList);

             IsRefreshing = false;*/
        }
        #endregion

        #region Methods
        public async void GetRoom()
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
            var response = await apiService.GetListWithCoockie<Room>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/room/list?sortedBy=name&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            roomList = (List<Room>)response.Result;
            Rooms = new ObservableCollection<Room>(roomList);
            IsRefreshing = false;
            if (Rooms.Count() == 0)
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
                return new RelayCommand(GetRoom);
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
                Rooms = new ObservableCollection<Room>(roomList);
            }
            else
            {
                Rooms = new ObservableCollection<Room>(
                    roomList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.name.ToLower().Contains(Filter.ToLower()) ||
                        l.description.ToLower().Contains(Filter.ToLower())));
            }

            if (Rooms.Count() == 0)
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
