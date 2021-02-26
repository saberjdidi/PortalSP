using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigurationPostedReport : PopupPage
    {
        public ConfigurationPostedReport()
        {
            InitializeComponent();
            BindingContext = new ConfigurationPostedReportViewModel();
        }
        private async void Close_Popup(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
        private async void Add_Configuration_Email(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewAddressPostedReport());
        }
        private async void Add_Configuration_JobCron(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewJobCronPostedReport());
        }
    }
}