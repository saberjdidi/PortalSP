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
   public class PostedReportViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<PostedReport> _postedReport;
        private bool isRefreshing;
        private string filter;
        private List<PostedReport> postedReportList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<PostedReport> PostedReports
        {
            get { return _postedReport; }
            set
            {
                _postedReport = value;
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
        public PostedReportViewModel()
        {
            apiService = new ApiServices();
            GetReports();
        }
        #endregion

        #region Methods
        public async void GetReports()
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
            string from = DateTime.Now.AddDays(-3).ToString("dd-MM-yyyy");
            string to = DateTime.Now.ToString("dd-MM-yyyy");
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.ListFromXmlToJson<PostedReport>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/postedReports/getPostedReports?from="+ from + "&to="+ to,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            if (response.Result == null)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Empty List", "ok");
                return;
            }
            // PostedReportResponse = (PostedReportResponse)response.Result;
            postedReportList = (List<PostedReport>)response.Result;
             PostedReports = new ObservableCollection<PostedReport>(postedReportList);
             IsRefreshing = false;
            /* if (PostedReports.Equals("<responseQuery><Result/></responseQuery>"))
             {
                 IsVisibleStatus = true;
             }
             else
             {
                 IsVisibleStatus = false;
             }*/
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetReports);
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
                PostedReports = new ObservableCollection<PostedReport>(postedReportList);
            }
            else
            {
                PostedReports = new ObservableCollection<PostedReport>(
                    postedReportList.Where(
                        l => l.Id.ToLower().Contains(Filter.ToLower())));
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
