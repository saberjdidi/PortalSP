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
    public partial class ConfigurationConventionPage : PopupPage
    {
        public ConfigurationConventionPage(Convention convention)
        {
            InitializeComponent();
            var viewModel = new ConfigurationConventionViewModel();
            viewModel.Convention = convention;
            BindingContext = viewModel;
        }

        private async void Close_Configuration(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}