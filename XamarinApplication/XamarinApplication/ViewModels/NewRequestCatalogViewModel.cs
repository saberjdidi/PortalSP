using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
    public class NewRequestCatalogViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewRequestCatalogViewModel()
        {
            apiService = new ApiServices();

            ListBranchAutoComplete();
            ListIcdoAutoComplete();
        }
        #endregion

        #region Properties
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
        public string Code { get; set; }
        public string Description { get; set; }
        public Branch Branch { get; set; }
        private Icdo _icdo = null;
        public Icdo Icdo
        {
            get { return _icdo; }
            set
            {
                _icdo = value;
                OnPropertyChanged();
            }
        }
        public Siapec Siapec { get; set; }
        public Nomenclatura Nomenclatura { get; set; }
        private bool _valid = false;
        public bool Valid
        {
            get { return _valid; }
            set
            {
                this._valid = value;
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
        private bool _showProcedure = false;
        public bool ShowProcedure
        {
            get => _showProcedure;
            set
            {
                _showProcedure = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void AddRequestCatatog()
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
            if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(Code))
            {
                Value = true;
                return;
            }
            if (Branch == null)
            {
                Value = true;
                await Application.Current.MainPage.DisplayAlert("Warning", "Branch is required", "ok");
                return;
            }
            if (Icdo == null)
            {
                Value = true;
                await Application.Current.MainPage.DisplayAlert("Warning", "Icdo is required", "ok");
                return;
            }
            if (Siapec == null)
            {
                Value = true;
                await Application.Current.MainPage.DisplayAlert("Warning", "Siapec is required", "ok");
                return;
            }
            if (Nomenclatura == null)
            {
                Value = true;
                await Application.Current.MainPage.DisplayAlert("Warning", "Nomenclatura is required", "ok");
                return;
            }
            var requestCatalog = new AddRequestCatalog
            {
                code = Code,
                description = Description,
                branch = Branch,
                icdo = Icdo,
                siapec = Siapec,
                nomenclatura = Nomenclatura,
                valid = Valid
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddRequestCatalog>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/requestCatalog/save",
            res,
            requestCatalog);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Request Catalog Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand SaveRequestCatatog
        {
            get
            {
                return new Command(() =>
                {
                    AddRequestCatatog();
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
                    //Navigation.PopAsync();
                    Debug.WriteLine("********Close*************");
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
        public ICommand OpenInterventionCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (Siapec == null)
                    {
                        Application.Current.MainPage.DisplayAlert("Alert", "Please Select Intervention", "ok");
                        return;
                    }
                    else
                    {
                        ListNomenclaturaAutoComplete();
                        ShowProcedure = true;
                    }
                });
            }
        }
        #endregion

        #region Autocomplete
        //Branch
        private List<Branch> _branchAutoComplete;
        public List<Branch> BranchAutoComplete
        {
            get { return _branchAutoComplete; }
            set
            {
                _branchAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Branch>> ListBranchAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                criteria1 = "",
                order = "asc",
                sortedBy = "name"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Branch>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/branch/search",
            res,
            _searchModel);
            BranchAutoComplete = (List<Branch>)response.Result;
            return BranchAutoComplete;
        }
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
        //Procedure(Nomenclature)
        private List<Nomenclatura> _nomenclatureAutoComplete;
        public List<Nomenclatura> NomenclaturAutoComplete
        {
            get { return _nomenclatureAutoComplete; }
            set
            {
                _nomenclatureAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Nomenclatura>> ListNomenclaturaAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                criteria0 = "",
                criteria1 = "",
                criteria2 = Siapec.code,
                order = "asc",
                sortedBy = "descrEsameProf"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Nomenclatura>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/nomenclatura/advancedSearch",
            res,
            _searchModel);
            NomenclaturAutoComplete = (List<Nomenclatura>)response.Result;
            return NomenclaturAutoComplete;
        }
        #endregion
    }
}
