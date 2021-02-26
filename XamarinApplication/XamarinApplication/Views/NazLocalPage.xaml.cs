using System;
using Rg.Plugins.Popup.Services;
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
    public partial class NazLocalPage : ContentPage
    {
        public NazLocalPage()
        {
            InitializeComponent();
            BindingContext = new NazLocalViewModel();
        }
        private async void Add_NazLocal(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewNazLocalPage());
        }
        private async void Update_NazLocal(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            NazLocal nazLocal = ((NazLocalViewModel)BindingContext).NazLocal.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new UpdateNazLocalPage(nazLocal));
        }
    }
}