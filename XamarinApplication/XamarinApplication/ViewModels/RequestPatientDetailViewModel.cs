using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
    public class RequestPatientDetailViewModel
    {
        #region Attribute
        public INavigation Navigation { get; set; }
        public Request RequestPatient { get; set; }
        #endregion

        #region Constructor
        public RequestPatientDetailViewModel()
        {
            
        }
        #endregion

        #region Commands
        public ICommand RequestExam
        {
            get
            {
                return new Command(async () =>
                {
                var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                //get list Report
                var getUrl = "https://portalesp.smart-path.it/Portalesp/report/preparePrintExam?requestId=" + RequestPatient.id;
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
                var url = "https://portalesp.smart-path.it/Portalesp/report/printExamReport?requestId=" + RequestPatient.id + "&reportId=" + getReport.Select(r => r.id).FirstOrDefault();
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

                        await DependencyService.Get<ISave>().SaveAndView(RequestPatient.code + "-" + dateNow + ".pdf", "application/pdf", stream);
                        await App.Current.MainPage.Navigation.PopPopupAsync(true);
                    }
                });
            }
        }
        public ICommand PrintAcceptation
        {
            get
            {
                return new Command(async () =>
                {
                    var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
                    var cookie = Settings.Cookie;
                    var res = cookie.Substring(11, 32);

                    var cookieContainer = new CookieContainer();
                    var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                    var client = new HttpClient(handler);
                    var url = "https://portalesp.smart-path.it/Portalesp/report/generateAcceptationReport?id=" + RequestPatient.id;
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

                        await DependencyService.Get<ISave>().SaveAndView(RequestPatient.patient.fullName + "-" + dateNow + ".DOCX", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", stream);
                        await App.Current.MainPage.Navigation.PopPopupAsync(true);
                    }
                });
            }
        }
        public ICommand NotePatient
        {
            get
            {
                return new Command(async () =>
                {
                    var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
                    var cookie = Settings.Cookie;
                    var res = cookie.Substring(11, 32);

                    var cookieContainer = new CookieContainer();
                    var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                    var client = new HttpClient(handler);
                    var url = "https://portalesp.smart-path.it/Portalesp/doctorAvis/printConsultation?id=" + RequestPatient.id;
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

                        await DependencyService.Get<ISave>().SaveAndView(RequestPatient.code + "-" + dateNow + ".pdf", "application/pdf", stream);
                        await App.Current.MainPage.Navigation.PopPopupAsync(true);
                    }
                });
            }
        }
        public ICommand PriliminaryReport
        {
            get
            {
                return new Command(async () =>
                {
                    var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
                    var cookie = Settings.Cookie;
                    var res = cookie.Substring(11, 32);
                    var _report = new PreliminaryReport
                    {
                        fromCheckList = true,
                        idReq = RequestPatient.id
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

                        await DependencyService.Get<ISave>().SaveAndView(RequestPatient.code + "-" + dateNow + ".pdf", "application/pdf", stream);
                        await App.Current.MainPage.Navigation.PopPopupAsync(true);
                    }
                });
            }
        }
        public ICommand BiologicalMaterials
        {
            get
            {
                return new Command(async () =>
                {
                    var cookie = Settings.Cookie;
                    var res = cookie.Substring(11, 32);

                    var cookieContainer = new CookieContainer();
                    var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                    var client = new HttpClient(handler);
                    var url = "https://portalesp.smart-path.it/Portalesp/request/generateMaterials?requestId=" + RequestPatient.id + "&requestCode=" + RequestPatient.code + "&patientFiscalCode=" + RequestPatient.patient.fiscalCode;
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

                        await DependencyService.Get<ISave>().SaveAndView("bioMaterials_request_" + RequestPatient.code + ".pdf", "application/pdf", stream);
                        await App.Current.MainPage.Navigation.PopPopupAsync(true);
                    }
                });
            }
        }
        public ICommand AmbulatoryRequest
        {
            get
            {
                return new Command(async () =>
                {
                    await PopupNavigation.Instance.PushAsync(new AmbulatoryRequestPage(RequestPatient));
                });
            }
        }
        public ICommand AttachmentRequest
        {
            get
            {
                return new Command(async () =>
                {
                    await PopupNavigation.Instance.PushAsync(new RequestAttachmentPage(RequestPatient));
                });
            }
        }
        #endregion
    }
}
