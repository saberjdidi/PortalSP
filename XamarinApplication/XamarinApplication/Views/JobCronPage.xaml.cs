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
    public partial class JobCronPage : TabbedPage
    {
        public JobCronPage()
        {
            InitializeComponent();
            BindingContext = new JobCronExpressionViewModel();
        }
        private async void New_JobCron(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewJobCronPage());
        }
        private async void New_Email(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewAddressMailPage());
        }
        private async void Update_JobCron(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var configs = mi.CommandParameter as Configs;
            await PopupNavigation.Instance.PushAsync(new UpdateJobCronPage(configs));
        }
        private async void Update_Email(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var addresses = mi.CommandParameter as Addresses;
            await PopupNavigation.Instance.PushAsync(new UpdateAddressMailPage(addresses));
        }
    }
}