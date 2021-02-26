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
   public class NewSymptomsViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewSymptomsViewModel()
        {
            apiService = new ApiServices();

            ListBranchAutoComplete();
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
        public string PTNMN { get; set; }
        public string Template { get; set; }
        private int _order = 5;
        public int Order
        {
            get { return _order; }
            set
            {
                this._order = value;
                OnPropertyChanged();
            }
        }
        private int _level = 1;
        public int Level
        {
            get { return _level; }
            set
            {
                this._level = value;
                OnPropertyChanged();
            }
        }
        private Branch _branch = null;
        public Branch Branch
        {
            get { return _branch; }
            set
            {
                _branch = value;
                OnPropertyChanged();
            }
        }
        private bool _hasSuccessor = true;
        public bool HasSuccessor
        {
            get { return _hasSuccessor; }
            set
            {
                this._hasSuccessor = value;
                OnPropertyChanged();
            }
        }
        private bool _preselected = false;
        public bool Preselected
        {
            get { return _preselected; }
            set
            {
                this._preselected = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void NewSymptoms()
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
            var symptoms = new AddSymptoms
            {
                esitCodi = Code,
                esitDesc = Description,
                esitPtnm = PTNMN,
                esitOrdi = Order,
                esitLive = Level,
                esitSucc = HasSuccessor,
                esitEspr = Preselected,
                esitTipoCodi = Branch,
                esitTest = "<!DOCTYPE html>↵<html>↵<head>↵</head>↵<body>↵<p>" + Template + "</p>↵</body>↵</html>"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddSymptoms>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/diagnosis/saveRootEsit?checklistId=14",
            res,
            symptoms);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Symptoms Added");
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
                    NewSymptoms();
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
        #endregion
    }
    #region Model
    public class AddSymptoms
    {
        public string esitCodi { get; set; }
        public string esitDesc { get; set; }
        public string esitPtnm { get; set; }
        public int esitLive { get; set; }
        public int esitOrdi { get; set; }
        public bool esitEspr { get; set; }
        public bool esitSucc { get; set; }
        public string esitTest { get; set; }
        //public string esitTipo { get; set; }
        public Branch esitTipoCodi { get; set; }
    }
    #endregion
}
