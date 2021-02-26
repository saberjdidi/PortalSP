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
    public class UpdateSIAPECViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Siapec _siapec;
        #endregion

        #region Constructors
        public UpdateSIAPECViewModel()
        {
            apiService = new ApiServices();
            ListBranchAutoComplete();
            ListCodRLAutoComplete();
        }
        #endregion

        #region Properties
        public Siapec Siapec
        {
            get { return _siapec; }
            set
            {
                _siapec = value;
                OnPropertyChanged();
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
        #endregion

        #region Methods
        public async void EditSIAPEC()
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
            if (string.IsNullOrEmpty(Siapec.code) || string.IsNullOrEmpty(Siapec.description) || Siapec.branch == null)
            {
                Value = true;
                return;
            }
            var siapec = new Siapec
            {
                id = Siapec.id,
                code = Siapec.code,
                description = Siapec.description,
                branch = Siapec.branch,
                codRL = Siapec.codRL,
                isExist = Siapec.isExist
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<Siapec>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/siapec/update",
            res,
            siapec);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            SiapecViewModel.GetInstance().Update(siapec);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "SIAPEC Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand UpdateSIAPEC
        {
            get
            {
                return new Command(() =>
                {
                    EditSIAPEC();
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
