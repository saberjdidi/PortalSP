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
    public partial class ServicePage : ContentPage
    {
        public ServicePage()
        {
            InitializeComponent();
            BindingContext = new ServiceViewModel();
        }
        private async void New_Service(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewServicePage());
        }
        private async void Service_Detail(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var ambulatory = mi.CommandParameter as Ambulatory;
            await PopupNavigation.Instance.PushAsync(new ServiceDetailsPage(ambulatory));
        }
        private async void Update_Service(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var ambulatory = mi.CommandParameter as Ambulatory;
            await PopupNavigation.Instance.PushAsync(new UpdateServicePage(ambulatory));
        }
    }
}