using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
    public class ReportsViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<Report> reports;
        private bool isRefreshing;
        private List<Report> reportsList;
        private bool isVisible;
        private string filter;
        private bool _showHide = false;
        //private Command<object> changeItemsSource;
        #endregion

        #region Properties
        public ObservableCollection<Report> Reports
        {
            get { return reports; }
            set
            {
                reports = value;
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
        public bool ShowHide
        {
            get => _showHide;
            set
            {
                _showHide = value;
                OnPropertyChanged();
            }
        }
        /*public Command<object> ChangeItemsSource
        {
            get { return changeItemsSource; }
            set { this.changeItemsSource = value; }
        }*/
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                this.isVisible = value;
                //this.RaisePropertyChanged("IsVisible");
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public ReportsViewModel()
        {
            apiService = new ApiServices();
            //isVisible = true;
            //ChangeItemsSource = new Command<object>(OnChangeItemsSource);
            GetReports();
        }
        #endregion
        /*private void OnChangeItemsSource(object obj)
        {
            if (IsVisible)
            {
                IsVisible = false;
                GetReports();
            }
            else
            {
               // GetReports.Clear();
                IsVisible = true;
            }
        }
        public void GetListOrEmpty()
        {
            if (IsVisible)
            {
                IsVisible = true;
            }
            else
            {
                
                IsVisible = false;
                GetReports();
            }
        }
             */
        #region Methods
        public async void GetReports()
        {
            IsRefreshing = true;
           // IsVisible = true;
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
            var _search = new SearchModel
            {
                criteria0 = "request",
                order = "asc",
                sortedBy = "name"
            };
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Report>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/report/search",
                 res,
                 _search);
            if (!response.IsSuccess)
            {
                //IsVisible = true;
                IsRefreshing = false;
              await Application.Current.MainPage.DisplayAlert("Warning", "List is Empty", "ok");
                return;
            }
            reportsList = (List<Report>)response.Result;
            Reports = new ObservableCollection<Report>(reportsList);
            IsRefreshing = false;
            if(Reports.Count() == 0)
            {
                IsVisible = true;
            }
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
                Reports = new ObservableCollection<Report>(reportsList);
            }
            else
            {
                Reports = new ObservableCollection<Report>(
                    reportsList.Where(
                        l => l.name.ToLower().StartsWith(Filter.ToLower())
                        || l.description.ToLower().StartsWith(Filter.ToLower())));
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
