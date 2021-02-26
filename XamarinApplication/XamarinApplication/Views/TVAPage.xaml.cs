using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TVAPage : ContentPage
    {
        public TVAPage()
        {
            InitializeComponent();
            BindingContext = new TVAViewModel();
        }
        private async void New_TVA(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewTVAPage());
        }
        private async void Update_TVA(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var tva = mi.CommandParameter as TVA;
            await PopupNavigation.Instance.PushAsync(new UpdateTVAPage(tva));
        }
        private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value)
            {
            }
            else
            {
                var cb = (CheckBox)sender;
                var tva = (TVA)cb.BindingContext;

                var cookie = Settings.Cookie;  
                var res = cookie.Substring(11, 32);

                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/tvaCode/setDefault?id=" + tva.id;
                client.BaseAddress = new Uri(url);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
                var response = await client.PostAsync(url, null);
                //listViewTva.IsRefreshing = true;
                var result = await response.Content.ReadAsStringAsync();
               // MessagingCenter.Send((App)Application.Current, "OnChecked");
                //await Task.Delay(3000);
                //listViewTva.IsRefreshing = false;
            }
        }
    }
}