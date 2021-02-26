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
    public partial class RequestCompilationPage : ContentPage
    {
        public RequestCompilationPage()
        {
            InitializeComponent();
            BindingContext = new RequestCompilationViewModel();
        }
        private async void New_RequestCompilation(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewRequestCompilationPage());
        }
        private async void Update_RequestCompilation(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var requestCompilation = mi.CommandParameter as RequestCompilation;
            //await PopupNavigation.Instance.PushAsync(new UpdateICDOPage(requestCompilation));
        }
    }
}