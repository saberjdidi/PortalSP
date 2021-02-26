using System;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using System.Windows.Input;

namespace XamarinApplication.ViewModels
{
    public class RequestAMBULATORYViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<Attachment> attachments;
        private bool isRefreshing;
        private string filter;
        private List<Attachment> attachmentsList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Attachment> Attachments
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
        public RequestAMBULATORYViewModel()
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
            var _searchModel = new SearchModel
            {
                criteria0 = "",
                criteria1 = "",
                criteria2 = "",
                criteria3 = "",
                criteria4 = "",
                criteria5 = "",
                id1 = -1,
                id2 = -1,
                nomenclatureId = -1,
                maxResult = 50,
                offset = 0,
                order = "desc",
                sortedBy = "service_send_date",
                status = ""
            };

            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            Debug.WriteLine("********cookie ViewModel*************");
            Debug.WriteLine(cookie);
            var response = await apiService.PostRequest<Attachment>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/ambulatoryRequest/search",
                 res,
                 _searchModel);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            attachmentsList = (List<Attachment>)response.Result;
            Attachments = new ObservableCollection<Attachment>(attachmentsList);
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
                Attachments = new ObservableCollection<Attachment>(attachmentsList);
            }
            else
            {
                Attachments = new ObservableCollection<Attachment>(
                    attachmentsList.Where(
                        l => l.patient.fullName.ToLower().Contains(Filter.ToLower()) ||
                             l.requests.Select(r => r.code).FirstOrDefault().ToLower().Contains(Filter.ToLower())));
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
