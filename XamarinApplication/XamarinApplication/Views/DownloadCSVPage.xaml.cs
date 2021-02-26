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
    public partial class DownloadCSVPage : ContentPage
    {
        public DownloadCSVPage()
        {
            InitializeComponent();
            BindingContext = new DownloadCSVViewModel();
        }
        private async void Download_CSV(object sender, EventArgs e)
        {
            //var mi = ((MenuItem)sender);
            //var csvFTP = mi.CommandParameter as CsvFTP;
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            CsvFTP csvFTP = ((DownloadCSVViewModel)BindingContext).Attachments.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/csvFTP/downloadCSVFile?id=" + csvFTP.id;
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

            await DependencyService.Get<ISave>().SaveAndView(csvFTP.name + ".csv", "application/CSV", stream);
        }
    }
}