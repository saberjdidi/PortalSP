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
    public partial class ClientPage : ContentPage
    {
        public ClientPage()
        {
            InitializeComponent();
            BindingContext = new ClientViewModel();
        }
        private async void Client_Reports(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var client = mi.CommandParameter as Client;
            await PopupNavigation.Instance.PushAsync(new ClientReportPage(client));
        }
    }
}