using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class RoomHistoricViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<RoomHistoric> _room;
        private List<RoomHistoric> roomList;
        bool _isVisibleStatus;
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructors
        public RoomHistoricViewModel()
        {
            apiService = new ApiServices();
            GetRoomHistoric();
        }
        #endregion

        #region Properties
        public Room Room { get; set; }
        public ObservableCollection<RoomHistoric> Rooms
        {
            get { return _room; }
            set
            {
                _room = value;
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
        #endregion

        #region Methods
        public async void GetRoomHistoric()
        {
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            var room = new Room
            {
                id = Room.id,
                code = Room.code,
                description = Room.description,
                active = Room.active,
                name = Room.name,
                createBy = Room.createBy,
                createDate = Room.createDate,
                deleteBy = Room.deleteBy,
                deleteDate = Room.deleteDate,
                deleteReason = Room.deleteReason
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.RoomHistoric<RoomHistoric>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/roomHistoric/getRoomHistoric",
            res,
            room);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            roomList = (List<RoomHistoric>)response.Result;
            Rooms = new ObservableCollection<RoomHistoric>(roomList);
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
        public Command ClosePopup
        {
            get
            {
                return new Command(() =>
                {
                    Navigation.PopPopupAsync();
                    //Navigation.PopAsync();
                    Debug.WriteLine("********Close*************");
                });
            }
        }
        #endregion
    }
}
