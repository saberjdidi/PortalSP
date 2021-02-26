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
   public class RoomActivateViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Properties
        public INavigation Navigation { get; set; }
        public Room Room { get; set; }
        #endregion

        #region Constructors
        public RoomActivateViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Methods
        public async void ActivateRoom()
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
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetAttachmentWithCoockie<Room>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/room/activate?id=" + Room.id,
                 res);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            //RequestsViewModel.GetInstance().Update(deleteRequest);
            await RoomViewModel.GetInstance().Activate(Room);
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Room Activated");
            await PopupNavigation.Instance.PopAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Activate
        {
            get
            {
                return new Command(() =>
                {
                    ActivateRoom();
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
