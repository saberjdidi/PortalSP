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
    public partial class DoctorPage : ContentPage
    {
        public DoctorPage()
        {
            InitializeComponent();
            BindingContext = new DoctorViewModel();
        }
        private async void Add_Doctor(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewDoctorPage());
        }
        private async void Doctor_Detail(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var doctor = mi.CommandParameter as Doctor;
            await PopupNavigation.Instance.PushAsync(new DoctorDetailPage(doctor));
        }
        private async void Update_Doctor(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var doctor = mi.CommandParameter as Doctor;
            await PopupNavigation.Instance.PushAsync(new UpdateDoctorPage(doctor));
        }
    }
}