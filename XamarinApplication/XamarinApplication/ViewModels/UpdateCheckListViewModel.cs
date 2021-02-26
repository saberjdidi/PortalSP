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
   public class UpdateCheckListViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private CheckList _checkList;
        #endregion

        #region Constructors
        public UpdateCheckListViewModel()
        {
            apiService = new ApiServices();

            ListBranchAutoComplete();
            ListIcdoAutoComplete();
        }
        #endregion

        #region Properties
        public CheckList CheckList
        {
            get { return _checkList; }
            set
            {
                _checkList = value;
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
        public async void EditCheckList()
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
            if (string.IsNullOrEmpty(CheckList.chlsCodi) || string.IsNullOrEmpty(CheckList.chlsDesc))
            {
                Value = true;
                return;
            }
            var checkList = new CheckList
            {
                id = CheckList.id,
                chlsCodi = CheckList.chlsCodi,
                chlsDesc = CheckList.chlsDesc,
                chlsMnem = CheckList.chlsMnem,
                chlsTipoCodi = CheckList.chlsTipoCodi,
                chlsTopoCodi = CheckList.chlsTopoCodi,
                chlsAtti = CheckList.chlsAtti,
                chlsObbl = CheckList.chlsObbl,
                chlsServCodi = CheckList.chlsServCodi,
                chlsVali = CheckList.chlsVali,
                chlsVisi = CheckList.chlsVisi
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<CheckList>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/diagnosis/update",
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
            CheckListViewModel.GetInstance().Update(checkList);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "CheckList Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Update
        {
            get
            {
                return new Command(() =>
                {
                    EditCheckList();
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
