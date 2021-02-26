using Newtonsoft.Json;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConsentDocumentPatient : PopupPage
    {
        public ConsentDocumentPatient(Patient patient)
        {
            InitializeComponent();
            var consentDocumentViewModel = new ConsentDocumentPatientViewModel();
            consentDocumentViewModel.Patient = patient;
            BindingContext = consentDocumentViewModel;
        }
        private async void Close_Popup(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
    public class DocumentConsentPatient
    {
        public string documentCode { get; set; }
        public string documentVersion { get; set; }
        public DocumentProperties documentProperties { get; set; }
    }
    public class DocumentProperties
    {
        public string data_signature_consensus { get; set; }
        public string data_place_date { get; set; }
        public string data_first_name { get; set; }
        public string data_birth_place { get; set; }
        public string data_last_name { get; set; }
        public string data_birth_date { get; set; }
        public string data_title_signed { get; set; } 
    }
}