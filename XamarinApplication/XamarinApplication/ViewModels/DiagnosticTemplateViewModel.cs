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
   public class DiagnosticTemplateViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<DiagnosticTemplate> _diagnosticTemplate;
        private bool isRefreshing;
        private string filter;
        private List<DiagnosticTemplate> diagnosticTemplateList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<DiagnosticTemplate> DiagnosticTemplates
        {
            get { return _diagnosticTemplate; }
            set
            {
                _diagnosticTemplate = value;
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
        public DiagnosticTemplateViewModel()
        {
            apiService = new ApiServices();
            GetDiagnosticTemplate();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetDiagnosticTemplate();
            });
        }
        #endregion

        #region Sigleton
        static DiagnosticTemplateViewModel instance;
        public static DiagnosticTemplateViewModel GetInstance()
        {
            if (instance == null)
            {
                return new DiagnosticTemplateViewModel();
            }

            return instance;
        }

        public void Update(DiagnosticTemplate template)
        {
            IsRefreshing = true;
            var oldtemplate = diagnosticTemplateList
                .Where(p => p.id == template.id)
                .FirstOrDefault();
            oldtemplate = template;
            DiagnosticTemplates = new ObservableCollection<DiagnosticTemplate>(diagnosticTemplateList);
            IsRefreshing = false;
        }
        public async Task Delete(DiagnosticTemplate template)
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
            var response = await apiService.Delete<DiagnosticTemplate>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/diagnosticTemplate/delete/" + template.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            diagnosticTemplateList.Remove(template);
            DiagnosticTemplates = new ObservableCollection<DiagnosticTemplate>(diagnosticTemplateList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetDiagnosticTemplate()
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
            var response = await apiService.GetListWithCoockie<DiagnosticTemplate>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/diagnosticTemplate/list?sortedBy=name&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            diagnosticTemplateList = (List<DiagnosticTemplate>)response.Result;
            DiagnosticTemplates = new ObservableCollection<DiagnosticTemplate>(diagnosticTemplateList);
            IsRefreshing = false;
            if (DiagnosticTemplates.Count() == 0)
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
                return new RelayCommand(GetDiagnosticTemplate);
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
                DiagnosticTemplates = new ObservableCollection<DiagnosticTemplate>(diagnosticTemplateList);
            }
            else
            {
                DiagnosticTemplates = new ObservableCollection<DiagnosticTemplate>(
                    diagnosticTemplateList.Where(
                        l => l.name.ToLower().Contains(Filter.ToLower()) ||
                        l.description.ToLower().Contains(Filter.ToLower())));
            }
            if (DiagnosticTemplates.Count() == 0)
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
