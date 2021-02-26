using System;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.ViewModels;
using XamarinApplication.Models;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IllnessEventPage : ContentPage
    {
        public IllnessEventPage()
        {
            InitializeComponent();
            BindingContext = new IllnessEventViewModel();
        }
        private async void Add_IllnessEvent(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewIllnessEventPage());
        }
        private async void Update_IllnessEvent(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            IllnessEvent illnessEvent = ((IllnessEventViewModel)BindingContext).IllnessEvents.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new UpdateIllnessEventPage(illnessEvent));
        }
    }
}