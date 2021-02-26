using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class DocumentViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<ConsentDocument> _document;
        private bool isRefreshing;
        private string filter;
        private List<ConsentDocument> documentList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<ConsentDocument> Documents
        {
            get { return _document; }
            set
            {
                _document = value;
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
        public DocumentViewModel()
        {
            apiService = new ApiServices();
            GetList();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetList();
            });
        }
        #endregion

        #region Sigleton
        static DocumentViewModel instance;
        public static DocumentViewModel GetInstance()
        {
            if (instance == null)
            {
                return new DocumentViewModel();
            }

            return instance;
        }

        public void Update(ConsentDocument document)
        {
            IsRefreshing = true;
            var olddocument = documentList
                .Where(p => p.id == document.id)
                .FirstOrDefault();
            olddocument = document;
            Documents = new ObservableCollection<ConsentDocument>(documentList);
            IsRefreshing = false;
        }
        public async Task Delete(ConsentDocument document)
        {
            IsRefreshing = true;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.Delete<ConsentDocument>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/document/delete/" + document.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            documentList.Remove(document);
            Documents = new ObservableCollection<ConsentDocument>(documentList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetList()
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
            var response = await apiService.GetListWithCoockie<ConsentDocument>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/document/list?sortedBy=code&order=asc&time="+ timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            documentList = (List<ConsentDocument>)response.Result;
            Documents = new ObservableCollection<ConsentDocument>(documentList);
            IsRefreshing = false;
            if (Documents.Count() == 0)
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
                return new RelayCommand(GetList);
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
                Documents = new ObservableCollection<ConsentDocument>(documentList);
            }
            else
            {
                Documents = new ObservableCollection<ConsentDocument>(
                    documentList.Where(
                        l => l.code.ToLower().StartsWith(Filter.ToLower()) ||
                        l.repositoryTemplate.documentVersion.description.ToLower().StartsWith(Filter.ToLower()) ||
                        l.repositoryTemplate.documentType.description.ToLower().StartsWith(Filter.ToLower())));
            }
            if (Documents.Count() == 0)
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
