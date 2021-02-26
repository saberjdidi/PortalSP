using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
   public class RequestHandledViewModel : BaseViewModel
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
        public RequestHandledViewModel()
        {
            apiService = new ApiServices();
            GetAttachments();
        }
        #endregion

        #region Methods
        public async void GetAttachments()
        {
            IsRefreshing = true;
            string from = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
            string to = DateTime.Now.ToString("yyyy-MM-dd");
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
            var _searchModel = new SearchExecutionReport
            {
                criteria1 = "",
                criteria3 = "request_creation_date",
                id3 = -1,
                nomenclatureId = -1,
                maxResult = 100,
                date = from,
                date1 = to,
                order = "desc",
                sortedBy = "request_creation_date",
                status = "ALL"
            };

            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            Debug.WriteLine("********cookie ViewModel*************");
            Debug.WriteLine(cookie);
            var response = await apiService.PostExecutionReport<Attachment>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/requestHandled/search",
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
                             l.requests.Select(r => r.client.companyName).FirstOrDefault().ToLower().Contains(Filter.ToLower()) ||
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
        public ICommand SearchPopup
        {
            get
            {
                return new Command(async () =>
                {
                    MessagingCenter.Subscribe<DialogResultAttachment>(this, "PopUpData", (value) =>
                    {
                        // string receivedData = value.RequestsPopup;
                        // MyLabel.Text = receivedData;
                        Attachments = value.AttachmentPopup;
                        if (Attachments.Count() == 0)
                        {
                            IsVisibleStatus = true;
                        }
                        else
                        {
                            IsVisibleStatus = false;
                        }
                    });
                    await PopupNavigation.Instance.PushAsync(new SearchAttachmentPage());
                });
            }
        }
        #endregion
    }
}
