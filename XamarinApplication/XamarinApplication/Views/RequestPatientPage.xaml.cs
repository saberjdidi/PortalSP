using Newtonsoft.Json;
using Plugin.DownloadManager;
using Plugin.DownloadManager.Abstractions;
using Rg.Plugins.Popup.Pages;
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
    public partial class RequestPatientPage : PopupPage
    {
       // public IDownloadFile File; //2 method
        //bool isDownloading = true; //2 method
        public RequestPatientPage(Patient patient)
        {
            InitializeComponent();
            var eventPatient = new RequestPatientViewModel();
            eventPatient.Patient = patient;
            BindingContext = eventPatient;

            //2 method
            CrossDownloadManager.Current.CollectionChanged += (sender, e) =>
            System.Diagnostics.Debug.WriteLine(
                "[DownloadManager] " + e.Action +
                " -> New items: " + (e.NewItems?.Count ?? 0) +
                " at " + e.NewStartingIndex +
                " || Old items: " + (e.OldItems?.Count ?? 0) +
                " at " + e.OldStartingIndex
                );
        }
        private async void Close_Request(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
        private async void Popup_Details(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Request request = ((RequestPatientViewModel)BindingContext).Requests.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
             await PopupNavigation.Instance.PushAsync(new RequestPatientDetailPopup(request));
            
        }
        /* private async void Request_Detail(object sender, EventArgs e)
         {
             var mi = ((MenuItem)sender);
             var requestPatient = mi.CommandParameter as RequestPatient;
             await PopupNavigation.Instance.PushAsync(new RequestPatientDetail(requestPatient));
         }*/
        private async void Print_Acceptation(object sender, EventArgs e)
        {
            // TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            // RequestPatient requestPatient = ((RequestPatientViewModel)BindingContext).Requests.Where(ser => ser.patient.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
           
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/report/generateAcceptationReport?id=3524";
           // var url = "https://portalesp.smart-path.it/Portalesp/report/generateAcceptationReport?id=" + requestPatient.requests.Select(r => r.id).FirstOrDefault();
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

                await DependencyService.Get<ISave>().SaveAndView("test" + ".DOCX", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", stream);
            }
        }
        private async void download_pdf(object sender, EventArgs e)
        {
            // TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            // RequestPatient requestPatient = ((RequestPatientViewModel)BindingContext).Requests.Where(ser => ser.patient.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
              var httpClient = new HttpClient();
              var url = "https://portalesp.smart-path.it/Portalesp/doctorAvis/printConsultation?id=3524";
              Debug.WriteLine("********url*************");
              Debug.WriteLine(url);
              var response = await httpClient.GetAsync(url);
              Debug.WriteLine("********url*************");
              Debug.WriteLine(url);
              var result = await response.Content.ReadAsStringAsync();
              Debug.WriteLine("********result*************");
              Debug.WriteLine(result);
              if (!response.IsSuccessStatusCode)
              {
                  await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                  return;
              }
            //var pdf = JsonConvert.DeserializeObject<PdfClient>(result);
            //StreamReader reader = new StreamReader(result);
            //string text = reader.ReadToEnd();
            byte[] output = new byte[result.Length];
            byte[] byteArray = Encoding.ASCII.GetBytes(result);
            byte[] bytes = Convert.FromBase64String(result);
              MemoryStream stream = new MemoryStream(bytes);

              await DependencyService.Get<ISave>().SaveAndView("patientrequest.pdf", "application/pdf", stream);

            /* var webClient = new WebClient();
             webClient.DownloadDataCompleted += (s, e) => {
                 var data = e.Result;
                 string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                 string localFilename = $"test.pdf";
                 File.WriteAllBytes(Path.Combine(documentsPath, localFilename), data);
                 Device.InvokeOnMainThreadAsync(() => {
                     Application.Current.MainPage.DisplayAlert("Done", "File downloaded and saved", null, "OK");
                    // new UIAlertView("Done", "File downloaded and saved", null, "OK", null).Show();
                 });
             };
             var url = new Uri("https://portalesp.smart-path.it/Portalesp/doctorAvis/printConsultation?id=3524");
             webClient.DownloadDataAsync(url);*/
            //2eme method
            // var Url = "https://info.sio2.be/tdtooo/sostdt.pdf";
            /// DownloadFile(Url);
        }
        private async void Request_Detail(object sender, ItemTappedEventArgs e)
        {
            var requestPatient = e.Item as Request;
            await PopupNavigation.Instance.PushAsync(new RequestPatientDetail(requestPatient));
        }


        //2 method
        /* public bool IsDownloading(IDownloadFile File)
         {
             if (File == null) return false;

             switch (File.Status)
             {
                 case DownloadFileStatus.INITIALIZED:
                 case DownloadFileStatus.PAUSED:
                 case DownloadFileStatus.PENDING:
                 case DownloadFileStatus.RUNNING:
                     return true;

                 case DownloadFileStatus.COMPLETED:
                 case DownloadFileStatus.CANCELED:
                 case DownloadFileStatus.FAILED:
                     return false;
                 default:
                     throw new ArgumentOutOfRangeException();
             }
         }
         public async void DownloadFile(String FileName)
         {
             await Task.Yield();
             await Task.Run(() =>
             {
                 var downloadManager = CrossDownloadManager.Current;
                 var file = downloadManager.CreateDownloadFile(FileName);
                 downloadManager.Start(file, true);

                 while (isDownloading)
                 {
                     isDownloading = IsDownloading(file);
                 }
             });

             if (!isDownloading)
             {
                 await DisplayAlert("File Status", "File Downloaded", "OK");
             }
         }
         public void AbortDownloading()
         {
             CrossDownloadManager.Current.Abort(File);
         }
         */
    }
}