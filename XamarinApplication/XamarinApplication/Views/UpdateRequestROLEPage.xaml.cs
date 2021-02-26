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
    public partial class UpdateRequestROLEPage : PopupPage
    {
        public UpdateRequestROLEPage(Attachment attachment)
        {
            InitializeComponent();
            var updateRequestViewModel = new UpdateRequestROLEViewModel();
            updateRequestViewModel.Attachment = attachment;
            BindingContext = updateRequestViewModel;

            var requestid = attachment.requests.Select(r => r.id).FirstOrDefault();
            MessagingCenter.Send(new PassIdPatient() { idPatient = requestid }, "UpdateRequestRole");
            var patientid = attachment.requests.Select(r => r.patient.id).FirstOrDefault();
            MessagingCenter.Send(new PassIdPatient() { idPatient = patientid }, "UpdatePatientId");
            var clientid = attachment.requests.Select(r => r.client.id).FirstOrDefault();
            MessagingCenter.Send(new PassIdPatient() { idPatient = clientid }, "UpdateClientId");
        }
    }
}