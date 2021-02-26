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
    public partial class NomenclaturePage : ContentPage
    {
        public NomenclaturePage()
        {
            InitializeComponent();
            BindingContext = new NomenclatureViewModel();
        }
        private async void Nomenclature_Detail(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var nomenclatura = mi.CommandParameter as Nomenclatura;
            await PopupNavigation.Instance.PushAsync(new NomenclatureDetail(nomenclatura));
        }
    }
}