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
   public class UpdateSymptomsViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Symptoms _symptoms;
        #endregion

        #region Constructors
        public UpdateSymptomsViewModel()
        {
            apiService = new ApiServices();
            ListBranchAutoComplete();
        }
        #endregion

        #region Properties
        public Symptoms Symptoms
        {
            get { return _symptoms; }
            set
            {
                _symptoms = value;
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
        public async void EditSymptoms()
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
            if (string.IsNullOrEmpty(Symptoms.data.esitCodi) || string.IsNullOrEmpty(Symptoms.data.esitDesc))
            {
                Value = true;
                return;
            }
            var symptoms = new UpdateSymptoms
            {
                id = Symptoms.data.id,
                esitCodi = Symptoms.data.esitCodi,
                esitDesc = Symptoms.data.esitDesc,
                esitColp = Symptoms.data.esitColp,
                esitDefa = Symptoms.data.esitDefa,
                esitEspr = Symptoms.data.esitEspr,
                esitSucc = Symptoms.data.esitSucc,
                esitLive = Symptoms.data.esitLive,
                esitPtnm = Symptoms.data.esitPtnm,
                esitOrdi = Symptoms.data.esitOrdi,
                esitTipo = Symptoms.data.esitTipo,
                esitTest = Symptoms.data.esitTest,
                esitTipoCodi = Symptoms.data.esitTipoCodi
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<UpdateSymptoms>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/diagnosis/updateRootEsit?oldCode="+ Symptoms.data.esitCodi + "&checklistId=14",
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
           // SymptomsViewModel.GetInstance().Update(Symptoms);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Symptoms Updated");
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
                    EditSymptoms();
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
    public class UpdateSymptoms
    {
        public int id { get; set; }
        public string esitCodi { get; set; }
        public string esitDesc { get; set; }
        public string esitPtnm { get; set; }
        public string esitColp { get; set; }
        public string esitDefa { get; set; }
        public string esitLive { get; set; }
        public int esitOrdi { get; set; }
        public bool esitEspr { get; set; }
        public bool esitSucc { get; set; }
        public string esitTest { get; set; }
        public string esitTipo { get; set; }
        public Branch esitTipoCodi { get; set; }
    }
    #endregion
}
