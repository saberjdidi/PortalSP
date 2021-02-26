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
    public partial class RequestPatientDetail : PopupPage
    {
        public RequestPatientDetail(Request requestPatient)
        {
            InitializeComponent();
            var requestPatientViewModel = new RequestPatientDetailViewModel();
            requestPatientViewModel.RequestPatient = requestPatient;
            BindingContext = requestPatientViewModel;
        }
        private async void Close_Popup(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}