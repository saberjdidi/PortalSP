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
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentPage : ContentPage
    {
        public PaymentPage()
        {
            InitializeComponent();
            BindingContext = new PaymentViewModel();
        }
        private async void New_Payment(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewPaymentPage());
        }
        private async void Update_Payment(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var payment = mi.CommandParameter as Payment;
            await PopupNavigation.Instance.PushAsync(new UpdatePaymentPage(payment));
        }
        private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value)
            {
            }
            else
            {
                var cb = (CheckBox)sender;
                var payment = (Payment)cb.BindingContext;

                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);

                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/payment/setDefault?id=" + payment.id;
                client.BaseAddress = new Uri(url);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
                var response = await client.PostAsync(url, null);
                var result = await response.Content.ReadAsStringAsync();
                //DependencyService.Get<INotification>().CreateNotification("CheckBox Changed", "Refresh Page !");
            }
        }

    }
}