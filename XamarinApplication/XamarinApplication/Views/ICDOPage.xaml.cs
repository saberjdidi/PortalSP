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
    public partial class ICDOPage : ContentPage
    {
        public ICDOPage()
        {
            InitializeComponent();
            BindingContext = new ICDOViewModel();
        }
        private async void New_Icdo(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewICDOPage());
        }
        private async void Update_Icdo(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var icdo = mi.CommandParameter as Icdo;
            await PopupNavigation.Instance.PushAsync(new UpdateICDOPage(icdo));
        }
    }
}