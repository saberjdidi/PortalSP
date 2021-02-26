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
    public partial class RequestCatalogPage : ContentPage
    {
        public RequestCatalogPage()
        {
            InitializeComponent();
            BindingContext = new RequestCatalogViewModel();
        }
        private async void Add_RequestCatalog(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewRequestCatalogPage());
        }
        private async void RequestCatalog_Detail(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var requestcatalog = mi.CommandParameter as Requestcatalog;
            await PopupNavigation.Instance.PushAsync(new RequestCatalogDetails(requestcatalog));
        }
        private async void Update_RequestCatalog(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var requestcatalog = mi.CommandParameter as Requestcatalog;
            await PopupNavigation.Instance.PushAsync(new UpdateRequestCatalogPage(requestcatalog));
        }
    }
}