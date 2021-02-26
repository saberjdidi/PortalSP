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
    public partial class DocumentDefinitionPage : ContentPage
    {
        public DocumentDefinitionPage()
        {
            InitializeComponent();
            BindingContext = new DocumentDefinitionViewModel();
        }
        private async void Add_Doctor(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewDoctorPage());
        }
        private async void Document_Detail(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var document = mi.CommandParameter as DocumentDefinition;
            await PopupNavigation.Instance.PushAsync(new DocumentDefinitionDetails(document));
        }
        private async void Update_Doctor(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var doctor = mi.CommandParameter as Doctor;
            await PopupNavigation.Instance.PushAsync(new UpdateDoctorPage(doctor));
        }
    }
}