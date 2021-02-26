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
    public partial class UpdateDoctorPage : PopupPage
    {
        public UpdateDoctorPage(Doctor doctor)
        {
            InitializeComponent();
            var viewModel = new UpdateDoctorViewModel();
            viewModel.DoctorId = doctor;
            BindingContext = viewModel;

            var userid = doctor.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = userid }, "UpdateDoctorId");
        }
    }
}