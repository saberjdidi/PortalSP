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
    public partial class SNOMEDPage : ContentPage
    {
        public SNOMEDPage()
        {
            InitializeComponent();
            BindingContext = new SNOMEDViewModel();
        }

        private async void New_SNOMED(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewSNOMEDPage());
        }
        private async void Update_SNOMED(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var snomed = mi.CommandParameter as Snomed;
            await PopupNavigation.Instance.PushAsync(new UpdateSNOMEDPage(snomed));
        }
    }
}