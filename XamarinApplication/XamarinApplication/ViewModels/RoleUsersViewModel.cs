using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class RoleUsersViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<User> _user;
        private List<User> userList;
        private bool isVisible;
        private bool isRefreshing;
        #endregion

        #region Properties
        public Role Role { get; set; }
        public ObservableCollection<User> Users
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
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
        #endregion

        #region Constructors
        public RoleUsersViewModel()
        {
            apiService = new ApiServices();
            GetUsers();
        }
        #endregion

        #region Methods
        public async void GetUsers()
        {
            IsRefreshing = true;
            var connection = await apiService.CheckConnection();
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);

            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }
            var _role = new Role
            {
                id = Role.id,
                authority = Role.authority,
                type = Role.type
            };
            var response = await apiService.PostRole<User>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/user/getUsresByRole",
                 res,
                 _role);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            userList = (List<User>)response.Result;
            IsRefreshing = false;
            if (userList == null)
            {
                IsVisible = true;
            }
            else
            {
                Users = new ObservableCollection<User>(userList);
                
                if (Users.Count() == 0)
                {
                    IsVisible = true;
                }
                else
                {
                    IsVisible = false;
                }
            }
            

        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetUsers);
            }
        }
        #endregion
    }
}
