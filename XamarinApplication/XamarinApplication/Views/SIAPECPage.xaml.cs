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
    public partial class SIAPECPage : ContentPage
    {
        public SIAPECPage()
        {
            InitializeComponent();
            BindingContext = new SiapecViewModel();
        }
        private async void New_SIAPEC(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewSIAPECPage());
        }
       private async void Update_SIAPEC(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var siapec = mi.CommandParameter as Siapec;
            await PopupNavigation.Instance.PushAsync(new UpdateSIAPECPage(siapec));
        }
    }
}