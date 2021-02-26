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
    public class DocumentDefinitionViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<DocumentDefinition> _documentDefinition;
        private bool isRefreshing;
        private string filter;
        private List<DocumentDefinition> documentList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<DocumentDefinition> Documents
        {
            get { return _documentDefinition; }
            set
            {
                _documentDefinition = value;
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
        public DocumentDefinitionViewModel()
        {
            apiService = new ApiServices();
            GetDocuments();
            instance = this;
        }
        #endregion

        #region Sigleton
        static DocumentDefinitionViewModel instance;
        public static DocumentDefinitionViewModel GetInstance()
        {
            if (instance == null)
            {
                return new DocumentDefinitionViewModel();
            }

            return instance;
        }

        public void Update(DocumentDefinition document)
        {
            IsRefreshing = true;
            var olddocument = documentList
                .Where(p => p.id == document.id)
                .FirstOrDefault();
            olddocument = document;
            Documents = new ObservableCollection<DocumentDefinition>(documentList);
            IsRefreshing = false;
        }
        public async Task Delete(DocumentDefinition document)
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
            var response = await apiService.Delete<DocumentDefinition>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/DocDefinition/delete/" + document.id + "?id=@id",
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
            Documents = new ObservableCollection<DocumentDefinition>(documentList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetDocuments()
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
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<DocumentDefinition>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/DocDefinition/list",
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            documentList = (List<DocumentDefinition>)response.Result;
            Documents = new ObservableCollection<DocumentDefinition>(documentList);
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
                return new RelayCommand(GetDocuments);
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
                Documents = new ObservableCollection<DocumentDefinition>(documentList);
            }
            else
            {
                Documents = new ObservableCollection<DocumentDefinition>(
                    documentList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.name.ToLower().Contains(Filter.ToLower())));
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
