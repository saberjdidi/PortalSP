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
    public class RequestHistoricViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<RequestHistoric> _requestHistoric;
        private bool isRefreshing;
        private string filter;
        private List<RequestHistoric> requestHistoricList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<RequestHistoric> RequestHistoric
        {
            get { return _requestHistoric; }
            set
            {
                _requestHistoric = value;
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
        MasterMenu _selectedMenu;
        public MasterMenu SelectedMenu
        {
            get
            {
                return _selectedMenu;
            }
            set
            {
                if (_selectedMenu != null)
                {
                    _selectedMenu.Selected = false;
                    _selectedMenu.MenuIcon = _selectedMenu.MenuIcon.Substring(0, _selectedMenu.MenuIcon.Length - 6);
                }


                _selectedMenu = value;

                if (_selectedMenu != null)
                {
                    _selectedMenu.Selected = true;
                    _selectedMenu.MenuIcon += "_color";
                    MessagingCenter.Send<MasterMenu>(_selectedMenu, "OpenMenu");
                }
            }
        }
        #endregion

        #region Constructors
        public RequestHistoricViewModel()
        {
            apiService = new ApiServices();
            GetRequestHistoric();
        }
        #endregion

        #region Methods
        public async void GetRequestHistoric()
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
            var response = await apiService.GetListWithCoockie<RequestHistoric>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/requestHistoric/list?sortedBy=requestCode&order=desc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            requestHistoricList = (List<RequestHistoric>)response.Result;
            RequestHistoric = new ObservableCollection<RequestHistoric>(requestHistoricList);
            IsRefreshing = false;
            if (RequestHistoric.Count() == 0)
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
                return new RelayCommand(GetRequestHistoric);
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
                RequestHistoric = new ObservableCollection<RequestHistoric>(requestHistoricList);
            }
            else
            {
                RequestHistoric = new ObservableCollection<RequestHistoric>(
                    requestHistoricList.Where(
                        l => l.patientFullName.ToLower().StartsWith(Filter.ToLower()) ||
                        l.clientCompanyName.ToLower().StartsWith(Filter.ToLower()) ||
                        l.requestCode.ToLower().StartsWith(Filter.ToLower())));
            }
            if (RequestHistoric.Count() == 0)
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
