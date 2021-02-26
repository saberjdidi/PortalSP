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
    public partial class NomenclatureRLPage : ContentPage
    {
        public NomenclatureRLPage()
        {
            InitializeComponent();
            BindingContext = new NomenclatureRLViewModel();
        }
        private async void New_NomenclatureRL(object sender, EventArgs e)
        {
             await PopupNavigation.Instance.PushAsync(new NewNomenclatureRLPage());
        }
        private async void Update_NomenclatureRL(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var nomenclatureRL = mi.CommandParameter as NomenclatureRL;
            await PopupNavigation.Instance.PushAsync(new UpdateNomenclatureRLPage(nomenclatureRL));
        }
    }
}