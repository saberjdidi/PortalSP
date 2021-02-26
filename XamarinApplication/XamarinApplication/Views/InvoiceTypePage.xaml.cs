using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class InvoiceTypePage : ContentPage
    {
        public InvoiceTypePage()
        {
            InitializeComponent();
            BindingContext = new InvoiceTypeViewModel();
        }

        protected override void OnAppearing()
        {
            (this.BindingContext as InvoiceTypeViewModel).GetInvoiceType();
        }

        private async void New_InvoiceType(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewInvoiceTypePage());
        }
        private async void Update_InvoiceType(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var invoice = mi.CommandParameter as InvoiceType;
            await PopupNavigation.Instance.PushAsync(new UpdateInvoiceTypePage(invoice));
        }
        private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value)
            {
            }
            else
            {
                var cb = (CheckBox)sender;
                var invoice = (InvoiceType)cb.BindingContext;
                Debug.WriteLine("**************invoice type****************");

                var cookie = Settings.Cookie;  //.Split(11, 33)
                var res = cookie.Substring(11, 32);

                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/electronicDocumentType/setDefault?id=" + invoice.id;
                client.BaseAddress = new Uri(url);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
                //var request = new HttpRequestMessage(HttpMethod.Post, url);
                //var response = await client.SendAsync(request);
                var response = await client.PostAsync(url, null);
                var result = await response.Content.ReadAsStringAsync();
                //InvoiceTypeViewModel.GetInstance().Update(invoice);
                //(this.BindingContext as InvoiceTypeViewModel).DataAfterChecked();
                /*
                 var response = await apiService.CheckBoxCheckChanged<InvoiceType>(
                      "https://portalesp.smart-path.it",
                      "/Portalesp",
                      "/electronicDocumentType/setDefault?id=" + invoice.id,
                      res);
                 if (!response.IsSuccess)
                 {
                     await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                     return;
                 }*/
            }
        }

        private async Task OpenAnimation(View view, uint length = 250)
        {
            view.RotationX = -90;
            view.IsVisible = true;
            view.Opacity = 0;
            _ = view.FadeTo(1, length);
            await view.RotateXTo(0, length);
        }

        private async Task CloseAnimation(View view, uint length = 250)
        {
            _ = view.FadeTo(0, length);
            await view.RotateXTo(-90, length);
            view.IsVisible = false;
        }
        private async void MainExpander_Tapped(object sender, EventArgs e)
        {
            var expander = sender as Expander;
            // var imgView = expander.FindByName<Grid>("ImageView");
            var detailsView = expander.FindByName<Grid>("DetailsView");

            if (expander.IsExpanded)
            {
                // await OpenAnimation(imgView);
                await OpenAnimation(detailsView);
            }
            else
            {
                await CloseAnimation(detailsView);
                // await CloseAnimation(imgView);
            }
        }
    }
}