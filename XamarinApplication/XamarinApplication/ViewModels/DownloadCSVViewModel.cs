using System;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using XamarinApplication.Models;
using XamarinApplication.Services;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;

namespace XamarinApplication.ViewModels
{
   public class DownloadCSVViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<CsvFTP> attachments;
        private bool isRefreshing;
        private string filter;
        private List<CsvFTP> attachmentsList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<CsvFTP> Attachments
        {
            get { return attachments; }
            set
            {
                attachments = value;
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
        public DownloadCSVViewModel()
        {
            apiService = new ApiServices();
            GetAttachments();
        }
        #endregion

        #region Methods
        public async void GetAttachments()
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
            var response = await apiService.GetListWithCoockie<CsvFTP>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/csvFTP/list?sortedBy=date&order=desc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            attachmentsList = (List<CsvFTP>)response.Result;
            Attachments = new ObservableCollection<CsvFTP>(attachmentsList);
            IsRefreshing = false;
            if (Attachments.Count() == 0)
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
                return new RelayCommand(GetAttachments);
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
                Attachments = new ObservableCollection<CsvFTP>(attachmentsList);
            }
            else
            {
                Attachments = new ObservableCollection<CsvFTP>(
                    attachmentsList.Where(
                        l => l.name.ToLower().Contains(Filter.ToLower())));
            }
            if (Attachments.Count() == 0)
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
