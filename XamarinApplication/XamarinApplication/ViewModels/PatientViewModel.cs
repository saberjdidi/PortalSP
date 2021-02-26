using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
    public class PatientViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Patient> patients;
        private bool isRefreshing;
        private string filter;
        private List<Patient> patientsList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
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
        public PatientViewModel()
        {
            apiService = new ApiServices();
            instance = this;
            GetPatients();

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetPatients();
            });
        }
        #endregion

        #region Sigleton
        static PatientViewModel instance;
        public static PatientViewModel GetInstance()
        {
            if (instance == null)
            {
                return new PatientViewModel();
            }

            return instance;
        }

        public void Update(Patient patient)
        {
            IsRefreshing = true;
            var oldPatient = patientsList
                .Where(p => p.id == patient.id)
                .FirstOrDefault();
            oldPatient = patient;
            Patients = new ObservableCollection<Patient>(patientsList);
            IsRefreshing = false;
        }
        public async Task Delete(Patient patient)
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
            var response = await apiService.Delete<Patient>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/patient/delete/" + patient.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            patientsList.Remove(patient);
            Patients = new ObservableCollection<Patient>(patientsList);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetPatients()
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
            var _search = new SearchModel
            {
                maxResult = 200,
                order = "asc",
                sortedBy = "lastName"
            };

            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            Debug.WriteLine("********cookie ViewModel*************");
            Debug.WriteLine(cookie);
            var response = await apiService.PostRequest<Patient>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/patient/search",
                 res,
                 _search);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            patientsList = (List<Patient>)response.Result;
            Patients = new ObservableCollection<Patient>(patientsList);
            IsRefreshing = false;
            if (Patients.Count() == 0)
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
                return new RelayCommand(GetPatients);
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
                Patients = new ObservableCollection<Patient>(patientsList);
            }
            else
            {
                Patients = new ObservableCollection<Patient>(
                    patientsList.Where(
                        l => l.fiscalCode.ToLower().StartsWith(Filter.ToLower()) ||
                        l.firstName.ToLower().Contains(Filter.ToLower()) ||
                        l.lastName.ToLower().Contains(Filter.ToLower())));
            }
            if (Patients.Count() == 0)
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
        public ICommand SearchPopup
        {
            get
            {
                return new Command(async () =>
                {
                    MessagingCenter.Subscribe<DialogResultPatient>(this, "PopUpData", (value) =>
                    {
                        // string receivedData = value.RequestsPopup;
                        // MyLabel.Text = receivedData;
                        Patients = value.PatientPopup;
                        if (Patients.Count() == 0)
                        {
                            IsVisibleStatus = true;
                        }
                        else
                        {
                            IsVisibleStatus = false;
                        }
                    });
                    await PopupNavigation.Instance.PushAsync(new SearchPatientPage());
                });
            }
        }
        #endregion
    }
}
