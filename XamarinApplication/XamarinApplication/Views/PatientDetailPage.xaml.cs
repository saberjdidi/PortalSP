using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class PatientDetailPage : PopupPage
    {
        public PatientDetailPage(Patient patient)
        {
            InitializeComponent();
            var viewModel = new PatientDetailViewModel();
            viewModel.PatientId = patient;
            BindingContext = viewModel;

            
            var patientid = patient.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = patientid }, "PatientId");
        }
        private async void Close_Popup_Patient(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
        private async void Consent_document(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
           // Patient patient = ((PatientViewModel)BindingContext).Patients.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            Patient patient = ((PatientDetailViewModel)BindingContext).PatientId;
            if(patient.isRepositorySaved == true)
            {
                await PopupNavigation.Instance.PushAsync(new ConsentDocumentPatientTrue(patient));
            }
            else
            {
            await PopupNavigation.Instance.PushAsync(new ConsentDocumentPatient(patient));
            }
        }
        private async void Patient_Slave(object sender, EventArgs e)
        {
           Patient patient = ((PatientDetailViewModel)BindingContext).PatientId;
            await PopupNavigation.Instance.PushAsync(new PatientSlavePage(patient));
        }

        private async void New_Request(object sender, EventArgs e)
        {
            Patient patient = ((PatientDetailViewModel)BindingContext).PatientId;
            await PopupNavigation.Instance.PushAsync(new NewRequestPage(patient));
        }
        /* private async void Patient_Update(object sender, EventArgs e)
        {
            Patient patient = ((PatientDetailViewModel)BindingContext).Patient;
            await Navigation.PushAsync(new UpdatePatientPage(patient));
        }*/
    }
    public class PassIdPatient
    {
        public int idPatient { get; set; }
    }
}