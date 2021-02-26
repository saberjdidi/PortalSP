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
    public partial class ConfigurationGlobaleConvention : PopupPage
    {
        public ConfigurationGlobaleConvention()
        {
            InitializeComponent();
            BindingContext = new ConfigurationGlobaleConventionViewModel();
        }
        private async void Add_Configuration_Global(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewConfigurationGlobalConvention());
        }
        private async void Update_Convention_Global(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            ConfigurationConvention convention = ((ConfigurationGlobaleConventionViewModel)BindingContext).ConfigConventions.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new UpdateConfigurationGlobalConfiguration(convention));
        }
        private async void Close_Configuration(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}