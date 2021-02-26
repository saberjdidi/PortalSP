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
   public class NewCheckListViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewCheckListViewModel()
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
        public string Mnemonic { get; set; }
        public Branch Branch { get; set; }
        public Icdo Icdo { get; set; }
        private bool _active = false;
        public bool Active
        {
            get { return _active; }
            set
            {
                this._active = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void AddCheckList()
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
            var checkList = new AddCheckList
            {
                chlsCodi = Code,
                chlsDesc = Description,
                chlsTipoCodi = Branch,
                chlsTopoCodi = Icdo,
                chlsAtti = Active,
                chlsMnem = Mnemonic
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddCheckList>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/diagnosis/save",
            res,
            checkList);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "CheckList Added");
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
                    AddCheckList();
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
        #endregion
    }
}
