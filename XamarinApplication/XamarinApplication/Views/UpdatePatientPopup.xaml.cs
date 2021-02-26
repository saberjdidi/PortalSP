using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.Models;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdatePatientPopup : PopupPage
    {
        public UpdatePatientPopup(Patient patient)
        {
            InitializeComponent();
            var updatePatientViewModel = new UpdatePatientPopupViewModel();
            updatePatientViewModel.PatientId = patient;
            BindingContext = updatePatientViewModel;

            var patientid = patient.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = patientid }, "UpdatePatientId");
        }
    }
}