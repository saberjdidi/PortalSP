using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Extensions;
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
   public class SearchPatientViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Constructors
        public SearchPatientViewModel()
        {
            apiService = new ApiServices();
            ListPatientAutoComplete();
        }
        #endregion

        #region Attributes
        public EventHandler<DialogResultPatient> OnDialogClosed;
        private ObservableCollection<Patient> patients;
        private List<Patient> patientsList;
        private bool isRefreshing;
        private SearchModel _searchModel;
        #endregion

        #region Properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FiscalCode { get; set; }
        public string BirthDate { get; set; }
        public ObservableCollection<Patient> Patients
        {
            get { return patients; }
            set
            {
                patients = value;
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
        public bool IsVisible { get; set; }
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
        public async void GetPatientSearch()
        {
            // IsRefreshing = true;
           /* if (FiscalCode != "^([A-Za-z]{6}[0-9lmnpqrstuvLMNPQRSTUV]{2}[abcdehlmprstABCDEHLMPRST]{1}[0-9lmnpqrstuvLMNPQRSTUV]{2}[A-Za-z]{1}[0-9lmnpqrstuvLMNPQRSTUV]{3}[A-Z]{1}$)|(([sS]{1}[tT]{1}[pP]{1})([0-9]{13}))$")
            {
                await Application.Current.MainPage.DisplayAlert("Error", "FiscalCode Invalid", "ok");
                return;
            }*/
            _searchModel = new SearchModel
                {
                    criteria1 = FirstName,
                    criteria2 = LastName,
                    criteria3 = FiscalCode,
                    date2 = BirthDate,
                    maxResult = 200,
                    order = "asc",
                    sortedBy = "lastName"
                };

            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Patient>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/patient/search",
                 res,
                 _searchModel);
            patientsList = (List<Patient>)response.Result;
            Patients = new ObservableCollection<Patient>(patientsList);
            if (Patients.Count() == 0)
            {
                IsVisible = true;
            }
            MessagingCenter.Send(new DialogResultPatient() { PatientPopup = Patients }, "PopUpData");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetPatientSearch);
            }
        }
        public ICommand SearchCommand
        {
            get
            {
                return new Command(() =>
                {
                    GetPatientSearch();
                });
            }
        }
        #endregion

        #region Autocomplete
        //Patient
        private List<Patient> _patientAutoComplete;
        public List<Patient> PatientAutoComplete
        {
            get { return _patientAutoComplete; }
            set
            {
                _patientAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Patient>> ListPatientAutoComplete()
        {
            var _search = new SearchModel
            {
                maxResult = 400,
                order = "asc",
                sortedBy = "lastName"
            };
            var cookie = Settings.Cookie;  
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Patient>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/patient/search",
                 res,
                 _search);
            PatientAutoComplete = (List<Patient>)response.Result;
            return PatientAutoComplete;
        }
        #endregion
    }
    #region Models
    public class DialogResultPatient
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ObservableCollection<Patient> PatientPopup { get; set; }
    }
    #endregion
}
