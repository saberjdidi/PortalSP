using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class NewReportCatalogViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewReportCatalogViewModel()
        {
            apiService = new ApiServices();
            SelectedSiapec = new List<Siapec>();
            ListIcdoAutoComplete();
            ListRagServiceAutoComplete();
        }
        #endregion

        #region Properties
        public Icdo Icdo { get; set; }
        public RagService RagService { get; set; }
        private object selectedSiapec;
        public object SelectedSiapec
        {
            get { return selectedSiapec; }
            set
            {
                selectedSiapec = value;
                // RaisePropertyChanged("SelectedItem");
            }
        }

        private bool value = false;
        public bool Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }
        private bool _showIntervention = false;
        public bool ShowIntervention
        {
            get => _showIntervention;
            set
            {
                _showIntervention = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void AddNewReport()
        {
            Value = true;
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            if (Icdo == null || RagService == null || SelectedSiapec == null)
            {
                Value = true;
                await Application.Current.MainPage.DisplayAlert("Warning", "Report is required", "ok");
                return;
            }
            var _report = new AddReportCatalog
            {
                icdo = Icdo,
                ragService = RagService,
                siapecs = SelectedSiapec
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddReportCatalog>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/reportCatalog/save",
            res,
            _report);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Report Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Save
        {
            get
            {
                return new Command(() =>
                {
                    AddNewReport();
                });
            }
        }
        public Command ClosePopup
        {
            get
            {
                return new Command(() =>
                {
                    Navigation.PopPopupAsync();
                });
            }
        }
        public ICommand OpenTopograficCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (Icdo == null)
                    {
                        Application.Current.MainPage.DisplayAlert("Alert", "Please Select Topografic", "ok");
                        return;
                    }
                    else
                    {
                        ListSiapecAutoComplete();
                        ShowIntervention = true;
                    }
                });
            }
        }
        #endregion

        #region Autocomplete
        //ICDO
        private List<Icdo> _icdoAutoComplete;
        public List<Icdo> ICDOAutoComplete
        {
            get { return _icdoAutoComplete; }
            set
            {
                _icdoAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Icdo>> ListIcdoAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                order = "asc",
                sortedBy = "description"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Icdo>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/icdo/search",
            res,
            _searchModel);
            ICDOAutoComplete = (List<Icdo>)response.Result;
            return ICDOAutoComplete;
        }
        //RagService
        private List<RagService> _ragServiceAutoComplete;
        public List<RagService> RagServiceAutoComplete
        {
            get { return _ragServiceAutoComplete; }
            set
            {
                _ragServiceAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<RagService>> ListRagServiceAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                order = "asc",
                sortedBy = "description"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<RagService>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/ragService/search",
            res,
            _searchModel);
            RagServiceAutoComplete = (List<RagService>)response.Result;
            return RagServiceAutoComplete;
        }
        //Intervention(siapec)
        private List<Siapec> _siapecAutoComplete;
        public List<Siapec> SIAPECAutoComplete
        {
            get { return _siapecAutoComplete; }
            set
            {
                _siapecAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Siapec>> ListSiapecAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                criteria2 = Icdo.code,
                order = "asc",
                sortedBy = "description"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Siapec>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/nomenclatura/getNomenclaturaSiapecs/search",
            res,
            _searchModel);
            SIAPECAutoComplete = (List<Siapec>)response.Result;
            return SIAPECAutoComplete;
        }
        #endregion
    }
}
