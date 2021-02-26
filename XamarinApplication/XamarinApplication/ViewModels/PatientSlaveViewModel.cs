using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
    public class PatientSlaveViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<Patient> _patientSlave;
        private List<Patient> patientList;
        private bool isVisible;
        #endregion

        #region Properties
        public Patient Patient { get; set; }
        public ObservableCollection<Patient> PatientSlave
        {
            get { return _patientSlave; }
            set
            {
                _patientSlave = value;
                OnPropertyChanged();
            }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public PatientSlaveViewModel()
        {
            apiService = new ApiServices();
            GetPatients();
        }
        #endregion

        #region Methods
        public async void GetPatients()
        {
            var connection = await apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }
            var cookie = Settings.Cookie; 
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<Patient>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/patient/displayPatientSlaves?patientId=" + Patient.id,
                 res);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            patientList = (List<Patient>)response.Result;
            PatientSlave = new ObservableCollection<Patient>(patientList);
            if (PatientSlave.Count() == 0)
            {
                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }

        }
        #endregion
    }
}
