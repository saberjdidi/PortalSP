using Newtonsoft.Json;
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
    public partial class RequestTECHNICALPage : ContentPage
    {
        public RequestTECHNICALPage()
        {
            InitializeComponent();
            BindingContext = new RequestDOCTORViewModel();
        }
        private async void Archive_Request(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            await PopupNavigation.Instance.PushAsync(new ArchiveRequestRolePage(attachment));
        }
        private async void Attachment_Request(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            await PopupNavigation.Instance.PushAsync(new RequestAttachmentRole(attachment));
        }
        private async void Historic_Request(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            await PopupNavigation.Instance.PushAsync(new RequestHistoricRole(attachment));
        }
        private async void Request_Exam(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            //get list Report
            var getUrl = "https://portalesp.smart-path.it/Portalesp/report/preparePrintExam?requestId=" + attachment.requests.Select(r => r.id).FirstOrDefault();
            client.BaseAddress = new Uri(getUrl);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var getResponse = await client.GetAsync(getUrl);
            if (!getResponse.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Error", getResponse.StatusCode.ToString(), "ok");
                return;
            }
            var getResult = await getResponse.Content.ReadAsStringAsync();
            var getReport = JsonConvert.DeserializeObject<List<Report>>(getResult, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            Debug.WriteLine("+++++++++++++++++++++++++list++++++++++++++++++++++++");
            Debug.WriteLine(getReport.Select(r => r.id).FirstOrDefault());
            //Download pdf
            var url = "https://portalesp.smart-path.it/Portalesp/report/printExamReport?requestId=" + attachment.requests.Select(r => r.id).FirstOrDefault() + "&reportId=" + getReport.Select(r => r.id).FirstOrDefault();
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
            var result = await response.Content.ReadAsStreamAsync();
            Debug.WriteLine("********result*************");
            Debug.WriteLine(result);
            using (var streamReader = new MemoryStream())
            {
                result.CopyTo(streamReader);
                byte[] bytes = streamReader.ToArray();
                MemoryStream stream = new MemoryStream(bytes);
                Debug.WriteLine("********stream*************");
                Debug.WriteLine(stream);
                if (stream == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Warning", "Data is Empty", "ok");
                    return;
                }

                await DependencyService.Get<ISave>().SaveAndView(attachment.requests.Select(r => r.code).FirstOrDefault() + "-" + dateNow + ".pdf", "application/pdf", stream);
            }
        }
    }
}