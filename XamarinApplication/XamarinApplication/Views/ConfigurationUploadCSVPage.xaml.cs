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
    public partial class ConfigurationUploadCSVPage : ContentPage
    {
        public ConfigurationUploadCSVPage()
        {
            InitializeComponent();
            BindingContext = new ConfigurationUploadCSVViewModel();
        }
        private async void Config_JobCron(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewConfigurationUploadCSV());
        }
    }
}