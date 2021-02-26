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
    public partial class ClientReportPage : PopupPage
    {
        public ClientReportPage(Client client)
        {
            InitializeComponent();
            var viewModel = new ClientReportViewModel();
            viewModel.Client = client;
            BindingContext = viewModel;

            var clientid = client.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = clientid }, "ClientId");
        }
        private async void Close_Popup(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}