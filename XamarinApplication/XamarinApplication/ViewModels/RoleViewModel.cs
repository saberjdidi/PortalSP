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
   public class RoleViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<Role> _role;
        private bool isRefreshing;
        private string filter;
        private List<Role> roleList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Role> Roles
        {
            get { return _role; }
            set
            {
                _role = value;
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
        public RoleViewModel()
        {
            apiService = new ApiServices();
            GetRole();
        }
        #endregion

        #region Methods
        public async void GetRole()
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
            var response = await apiService.GetListWithCoockie<Role>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/role/list?sortedBy=authority&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            roleList = (List<Role>)response.Result;
            Roles = new ObservableCollection<Role>(roleList);
            IsRefreshing = false;
            if (Roles.Count() == 0)
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
                return new RelayCommand(GetRole);
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
                Roles = new ObservableCollection<Role>(roleList);
            }
            else
            {
                Roles = new ObservableCollection<Role>(
                    roleList.Where( l => l.authority.ToLower().Contains(Filter.ToLower())));
            }
            if (Roles.Count() == 0)
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
