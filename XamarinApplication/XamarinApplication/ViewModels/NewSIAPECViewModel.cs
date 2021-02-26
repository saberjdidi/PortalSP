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
    public class NewSIAPECViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewSIAPECViewModel()
        {
            apiService = new ApiServices();
            ListBranchAutoComplete();
            ListCodRLAutoComplete();

        }
        #endregion

        #region Properties
        public string Code { get; set; }
        public string Description { get; set; }
        public Branch Branch { get; set; }
        public CodRL CodRL { get; set; }

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
        #endregion

        #region Methods
        public async void AddNewSIAPEC()
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
            if (string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(Description))
            {
                Value = true;
                return;
            }
            if(Branch == null)
            {
                Value = true;
                await Application.Current.MainPage.DisplayAlert("Warning", "Branch is required", "ok");
                return;
            }
            var _icdo = new AddSiapec
            {
                code = Code,
                description = Description,
                branch = Branch,
                codRL = CodRL
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddSiapec>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/siapec/save",
            res,
            _icdo);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "SIAPEC Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand SaveSIAPEC
        {
            get
            {
                return new Command(() =>
                {
                    AddNewSIAPEC();
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
                id1 = 166,
                order = "asc",
                sortedBy = "name"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Branch>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/branch/getConfiguredBranches",
            res,
            _searchModel);
            BranchAutoComplete = (List<Branch>)response.Result;
            return BranchAutoComplete;
        }
        //CodRL
        private List<CodRL> _codRLAutoComplete;
        public List<CodRL> CodRLAutoComplete
        {
            get { return _codRLAutoComplete; }
            set
            {
                _codRLAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<CodRL>> ListCodRLAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                order = "desc",
                sortedBy = "code"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<CodRL>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/nomenclatureRL/search",
            res,
            _searchModel);
            CodRLAutoComplete = (List<CodRL>)response.Result;
            return CodRLAutoComplete;
        }
        #endregion
    }
}
