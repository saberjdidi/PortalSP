using Rg.Plugins.Popup.Pages;
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
    public partial class UpdateRequestPage : PopupPage
    {
        public UpdateRequestPage(Request request)
        {
            InitializeComponent();
            var updateRequestViewModel = new UpdateRequestViewModel();
            updateRequestViewModel.RequestId = request;
            BindingContext = updateRequestViewModel;

            var requestid = request.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = requestid }, "UpdateRequestId");
            var patientid = request.patient.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = patientid }, "UpdatePatientId");
            var clientid = request.client.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = clientid }, "UpdateClientId");

            /* if(request.doctorNoRef == null)
             {
                 return;
             }
             else
             {
                 var doctorNoRefid = request.doctorNoRef.client.id;
                 MessagingCenter.Send(new PassIdPatient() { idPatient = doctorNoRefid }, "DoctorNoRefId");
                 Debug.WriteLine("********Id of DoctorRef*************");
                 Debug.WriteLine(doctorNoRefid);
             }*/

        }
    }
}