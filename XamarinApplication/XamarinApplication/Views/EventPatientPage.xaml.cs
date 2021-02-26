using Rg.Plugins.Popup.Pages;
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
    public partial class EventPatientPage : PopupPage
    {
        public EventPatientPage(Patient patient)
        {
            InitializeComponent();
            var eventPatient = new EventPatientViewModel();
            eventPatient.Patient = patient;
            BindingContext = eventPatient;
        }
        private async void Close_Event(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}