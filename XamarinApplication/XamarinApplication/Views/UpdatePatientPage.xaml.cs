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
    public partial class UpdatePatientPage : TabbedPage
    {
        public UpdatePatientPage(Patient patient)
        {
            InitializeComponent();
            var updatePatientViewModel = new UpdatePatientViewModel(Navigation);
            updatePatientViewModel.PatientId = patient;
            BindingContext = updatePatientViewModel;

            NavigationPage.SetHasNavigationBar(this, false);  // Hide nav bar

            var patientid = patient.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = patientid }, "UpdatePatientId");
        }
    }
}