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
    public partial class TaxRegimePage : ContentPage
    {
        public TaxRegimePage()
        {
            InitializeComponent();
            BindingContext = new TaxRegimeViewModel();
        }
        private async void New_TaxRegime(object sender, EventArgs e)
        {
              await PopupNavigation.Instance.PushAsync(new NewTaxRegimePage());
        }
        private async void Update_TaxRegime(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var taxRegime = mi.CommandParameter as TaxRegime;
            await PopupNavigation.Instance.PushAsync(new UpdateTaxRegimePage(taxRegime));
        }
    }
}