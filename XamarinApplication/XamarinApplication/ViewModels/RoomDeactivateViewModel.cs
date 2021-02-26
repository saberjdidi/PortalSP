using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class RoomDeactivateViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Properties
        public INavigation Navigation { get; set; }
        public Room Room { get; set; }
        public string Reason { get; set; }
        private bool value = false;
        public bool Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public RoomDeactivateViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Methods
        public async void deleteRoom()
        {
            Value = true;
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            if (string.IsNullOrEmpty(Reason))
            {
                Value = true;
                return;
            }
            var room = new RoomDeactivate
            {
                id = Room.id,
                reason = Reason
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<RoomDeactivate>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/room/deactivate",
            res,
            room);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            await RoomViewModel.GetInstance().Activate(Room);
            //RequestsViewModel.GetInstance().Update(deleteRequest);
            Value = false;
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Room Deactivated");
            //await App.Current.MainPage.Navigation.PopPopupAsync(true);
            await PopupNavigation.Instance.PopAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Deactivate
        {
            get
            {
                return new Command(() =>
                {
                    deleteRoom();
                });
            }
        }
        public Command ClosePopup
        {
            get
            {
                return new Command(() =>
                {
                    Navigation.PopPopupAsync();
                });
            }
        }
        #endregion
    }
}
