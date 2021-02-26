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
    public partial class PatientPage : ContentPage
    {
        public PatientPage()
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
        private async void Event_Patient(object sender, EventArgs e)
        {
            // var mi = ((MenuItem)sender);
            // var patient = mi.CommandParameter as Patient;
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Patient patient = ((PatientViewModel)BindingContext).Patients.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new EventPatientPage(patient));
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
        private async void Patient_Signature(object sender, EventArgs e)
        {
            //var mi = ((MenuItem)sender);
            //var patient = mi.CommandParameter as Patient;
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Patient patient = ((PatientViewModel)BindingContext).Patients.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            //await PopupNavigation.Instance.PushAsync(new PatientDetailPage(patient));
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/patient/addSignature?patientId=" + patient.id;
            Debug.WriteLine("********url*************");
            Debug.WriteLine(url);
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");

            }
            var result = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("********result price*************");
            Debug.WriteLine(result);
        }
    }
}