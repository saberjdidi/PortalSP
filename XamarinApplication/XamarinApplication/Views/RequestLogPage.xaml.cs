using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    public partial class RequestLogPage : ContentPage
    {
        public RequestLogPage()
        {
            InitializeComponent();
            BindingContext = new RequestLogViewModel();
           // NavigationPage.SetHasNavigationBar(this, false);
        }
        private async void Download_CSVFile(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            RequestLog requestLog = ((RequestLogViewModel)BindingContext).RequestLog.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/csvFile/downloadCSVFile?id=" + requestLog.id;
            Debug.WriteLine("********url*************");
            Debug.WriteLine(url);
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                return;
            }
            var result = await response.Content.ReadAsByteArrayAsync();
            Debug.WriteLine("********result*************");
            Debug.WriteLine(result);
            /* var toBase64 = System.Convert.ToBase64String(result);
             byte[] bytes = Convert.FromBase64String(toBase64);
             Debug.WriteLine("********bytes*************");
             Debug.WriteLine(bytes);*/
            MemoryStream stream = new MemoryStream(result);
            Debug.WriteLine("********stream*************");
            Debug.WriteLine(stream);
            if (stream == null)
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Data is Empty", "ok");
                return;
            }

            await DependencyService.Get<ISave>().SaveAndView(requestLog.fileName + ".csv", "application/CSV", stream);
        }
        private async void Historic_RequestLog(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            RequestLog requestLog = ((RequestLogViewModel)BindingContext).RequestLog.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new RequestLogHistoricPage(requestLog));
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
            var detailsView = expander.FindByName<Grid>("DetailsView");

            if (expander.IsExpanded)
            {
                await OpenAnimation(detailsView);
            }
            else
            {
                await CloseAnimation(detailsView);
            }
        }
    }
}