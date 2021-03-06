﻿using Newtonsoft.Json;
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
    public partial class RequestADMINPage : ContentPage
    {
        public RequestADMINPage()
        {
            InitializeComponent();
            BindingContext = new RequestADMINViewModel();
        }
        private async void Request_Detail(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            //Attachment attachment = ((RequestADMINViewModel)BindingContext).Attachments.Where(ser => ser.requests.Select(r => r.id).FirstOrDefault() == (int)tappedEventArgs.Parameter).FirstOrDefault();
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new RequestDetailPage(request));
        }
        private async void Update_Request(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            refreshView.IsRefreshing = true;
            await PopupNavigation.Instance.PushAsync(new UpdateRequestPage(request));
            refreshView.IsRefreshing = false;
        }
        private async void Delete_Request(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            refreshView.IsRefreshing = true;
            await PopupNavigation.Instance.PushAsync(new DeleteRequestPage(request));
            refreshView.IsRefreshing = false;
        }
        private async void Attachment_Request(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            refreshView.IsRefreshing = true;
            await PopupNavigation.Instance.PushAsync(new RequestAttachmentPage(request));
            refreshView.IsRefreshing = false;
        }
        private async void Ambulatory_Request(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            
            await PopupNavigation.Instance.PushAsync(new AmbulatoryRequestPage(request));
        }
        private async void CheckList_Request(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();

            await PopupNavigation.Instance.PushAsync(new RequestCheckListPage(request));
        }
        private async void Request_Archive(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new ArchiveRequestPage(request));
        }
        private async void Biological_Material(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            refreshView.IsRefreshing = true;

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/request/generateMaterials?requestId=" + request.id + "&requestCode=" + request.code + "&patientFiscalCode=" + request.patient.fiscalCode;
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
            refreshView.IsRefreshing = false;
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

                await DependencyService.Get<ISave>().SaveAndView("bioMaterials_request_" + request.code + ".pdf", "application/pdf", stream);
            }
        }
        private async void Request_Exam(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            refreshView.IsRefreshing = true;
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            //get list Report
            var getUrl = "https://portalesp.smart-path.it/Portalesp/report/preparePrintExam?requestId=" + request.id;
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
            var url = "https://portalesp.smart-path.it/Portalesp/report/printExamReport?requestId=" + request.id + "&reportId=" + getReport.Select(r => r.id).FirstOrDefault();
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
            refreshView.IsRefreshing = false;
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

                await DependencyService.Get<ISave>().SaveAndView(request.code + "-" + dateNow + ".pdf", "application/pdf", stream);
            }
        }
        private async void Print_Acceptation(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            refreshView.IsRefreshing = true;

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/report/generateAcceptationReport?id=" + request.id;
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
            refreshView.IsRefreshing = false;
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

                await DependencyService.Get<ISave>().SaveAndView(request.patient.fullName + "-" + dateNow + ".DOCX", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", stream);
            }
        }
        private async void Note_Patient(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            refreshView.IsRefreshing = true;

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/doctorAvis/printConsultation?id=" + request.id;
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
            refreshView.IsRefreshing = false;
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

                await DependencyService.Get<ISave>().SaveAndView(request.code + "-" + dateNow + ".pdf", "application/pdf", stream);
            }
        }
        private async void Preliminary_Report(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestADMINViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            refreshView.IsRefreshing = true;
            var _report = new PreliminaryReport
            {
                fromCheckList = true,
                idReq = request.id
            };

            var requestJson = JsonConvert.SerializeObject(_report);
            Debug.WriteLine("********request*************");
            Debug.WriteLine(requestJson);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/doctorAvis/printReportTrackerFromTemplate";
            Debug.WriteLine("********url*************");
            Debug.WriteLine(url);
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                return;
            }
            var result = await response.Content.ReadAsStreamAsync();
            refreshView.IsRefreshing = false;
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

                await DependencyService.Get<ISave>().SaveAndView(request.code + "-" + dateNow + ".pdf", "application/pdf", stream);
            }
        }

    }
}