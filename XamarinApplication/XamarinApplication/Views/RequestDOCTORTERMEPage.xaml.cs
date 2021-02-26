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
    public partial class RequestDOCTORTERMEPage : ContentPage
    {
        public RequestDOCTORTERMEPage()
        {
            InitializeComponent();
            BindingContext = new RequestDOCTORTERMEViewModel();
        }
        private async void Delete_Request(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            refreshView.IsRefreshing = true;
            await PopupNavigation.Instance.PushAsync(new DeleteRequestROLEPage(attachment));
            refreshView.IsRefreshing = false;
        }
        private async void Update_Request(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            refreshView.IsRefreshing = true;
            await PopupNavigation.Instance.PushAsync(new UpdateRequestROLEPage(attachment));
            refreshView.IsRefreshing = false;
        }
        private async void Archive_Request(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            await PopupNavigation.Instance.PushAsync(new ArchiveRequestRolePage(attachment));
        }
        private async void Ambulatory_Request(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            await PopupNavigation.Instance.PushAsync(new AmbulatoryRequestRole(attachment));
        }
        private async void CheckList_Request(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            await PopupNavigation.Instance.PushAsync(new RequestCheckListRole(attachment));
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
            refreshView.IsRefreshing = true;
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
                refreshView.IsRefreshing = false;
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
                refreshView.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                return;
            }
            var result = await response.Content.ReadAsStreamAsync();
            Debug.WriteLine("********result*************");
            Debug.WriteLine(result);
            refreshView.IsRefreshing = false;
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

                await DependencyService.Get<ISave>().SaveAndView(attachment.requests.Select(r => r.code).FirstOrDefault() +"-"+ dateNow + ".pdf", "application/pdf", stream);
            }
        }
        private async void Biological_Material(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            refreshView.IsRefreshing = true;
            var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/request/generateMaterials?requestId=" + attachment.requests.Select(r => r.id).FirstOrDefault() + "&requestCode=" + attachment.requests.Select(r => r.code).FirstOrDefault() + "&patientFiscalCode=" + attachment.requests.Select(r => r.patient.fiscalCode).FirstOrDefault();
            Debug.WriteLine("********url*************");
            Debug.WriteLine(url);
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                refreshView.IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                return;
            }
            var result = await response.Content.ReadAsStreamAsync();
            Debug.WriteLine("********result*************");
            Debug.WriteLine(result);
            refreshView.IsRefreshing = false;
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

                await DependencyService.Get<ISave>().SaveAndView("bioMaterials_request_" + attachment.requests.Select(r => r.code).FirstOrDefault() + ".pdf", "application/pdf", stream);
            }
        }
        private async void Cancel_Tacking_Charge(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            if (attachment.requests.Select(r => r.isHandled).FirstOrDefault() == false)
            {
                await Application.Current.MainPage.DisplayAlert("Alert", "Request is canceled", "ok");
                return;
            }
            else
            {
                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);
                refreshView.IsRefreshing = true;

                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/request/cancelHandled?requestId=" + attachment.requests.Select(r => r.id).FirstOrDefault();
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
                var result = await response.Content.ReadAsStringAsync();
                refreshView.IsRefreshing = false;
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
            }
        }
    }
}