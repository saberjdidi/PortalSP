using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class UpdateRoomViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Room _room;
        #endregion

        #region Constructors
        public UpdateRoomViewModel()
        {
            apiService = new ApiServices();
            ListType = GetType().OrderBy(t => t.Value).ToList();
        }
        #endregion

        #region Properties
        public Room Room
        {
            get { return _room; }
            set
            {
                _room = value;
                OnPropertyChanged();
            }
        }
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

        #region Methods
        public async void EditRoom()
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
            if (string.IsNullOrEmpty(Room.code) || string.IsNullOrEmpty(Room.name))
            {
                Value = true;
                return;
            }
            if (SelectedType == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please Select Type", "ok");
                Value = true;
                return;
            }
            var room = new RoomUpdate
            {
                id = Room.id,
                active = Room.active,
                code = Room.code,
                name = Room.name,
                description = Room.description,
                type = SelectedType.Key,
                createBy = Room.createBy,
                deleteBy = Room.deleteBy,
                //createDate = Room.createDate.ToString(),
                //deleteDate = Room.deleteDate.ToString(),
                deleteReason = Room.deleteReason
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<RoomUpdate>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/room/update",
            res,
            room);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
           /* if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }*/
            Value = false;
            RoomViewModel.GetInstance().Update(Room);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Room Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Update
        {
            get
            {
                return new Command(() =>
                {
                    EditRoom();
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
                    //Navigation.PopAsync();
                    Debug.WriteLine("********Close*************");
                });
            }
        }
        #endregion

        #region type
        public List<Language> ListType { get; set; }
        private Language _selectedType { get; set; }
        public Language SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<Language> GetType()
        {
            var languages = new List<Language>()
            {
                new Language(){Key =  "SU", Value= "Surgery"},
                new Language(){Key =  "DI", Value= "Diagnostics"},
                new Language(){Key =  "OT", Value= "Other"}
            };

            return languages;
        }
        #endregion
    }
}
