using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using Syncfusion.SfPicker.XForms;
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
    public partial class NewRequestPage : PopupPage
    {
        //SfPicker picker;
        public NewRequestPage(Patient patient)
        {
            InitializeComponent();
            var newRequestViewModel = new NewRequestViewModel();
            newRequestViewModel.Patient = patient;
            BindingContext = newRequestViewModel;

            
            if(patient.client != null)
            {
                var clientid = patient.client.id;
                MessagingCenter.Send(new PassIdPatient() { idPatient = clientid }, "ClientId");
            } else
            {
                return;
            }

            var patientid = patient.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = patientid }, "PatientId");
        }
        /* private void searchEntry_TextChanged(object sender, TextChangedEventArgs e)
         {
             if (e.NewTextValue.Length == 2)
             {
                 foreach (var item in picker.ItemsSource)
                 {
                     if (item.ToString().Equals(e.NewTextValue.ToString(), StringComparison.OrdinalIgnoreCase))
                     {
                         picker.SelectedItem = item;
                     }
                 }
             }

         }

         private void picker1_OnPickerItemLoaded(object sender, PickerViewEventArgs e)
         {
             picker = sender as SfPicker;
         }
         private async void Close_Popup(object sender, EventArgs e)
         {
             await PopupNavigation.Instance.PopAsync(true);
         }*/
    }
}