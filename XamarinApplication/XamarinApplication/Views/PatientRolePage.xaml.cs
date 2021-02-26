using Rg.Plugins.Popup.Services;
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
    public partial class PatientRolePage : ContentPage
    {
        public PatientRolePage()
        {
            InitializeComponent();
            BindingContext = new PatientViewModel();
        }
        private async void Patient_Detail(object sender, EventArgs e)
        {
            //var mi = ((MenuItem)sender);
            //var patient = mi.CommandParameter as Patient;
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Patient patient = ((PatientViewModel)BindingContext).Patients.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new PatientDetailPage(patient));
        }
        private async void Request_Patient(object sender, EventArgs e)
        {
            // var mi = ((MenuItem)sender);
            // var patient = mi.CommandParameter as Patient;
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Patient patient = ((PatientViewModel)BindingContext).Patients.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new RequestPatientPage(patient));
        }
        private async void Update_Patient(object sender, EventArgs e)
        {
            //var mi = ((MenuItem)sender);
            //var patient = mi.CommandParameter as Patient;
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Patient patient = ((PatientViewModel)BindingContext).Patients.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new UpdatePatientPopup(patient));
        }
        private async void Add_Patient(object sender, EventArgs e)
        {
            //await PopupNavigation.Instance.PushAsync(new NewPatientPage());
            await Navigation.PushAsync(new NewPatientPage());
        }
    }
}