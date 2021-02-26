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
    public partial class ExecutionReportPage : ContentPage
    {
        public ExecutionReportPage()
        {
            InitializeComponent();
            BindingContext = new ExecutionReportViewModel();
        }
        private async void Download_Excel(object sender, EventArgs e)
        {
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var dateNow = DateTime.Now.ToString("dd-MM-yyyy");

            var _searchModel = new SearchExecutionReport
            {
                criteria1 = "",
                criteria3 = "",
                date = null,
                date1 = null,
                id3 = -1,
                maxResult = 50,
                nomenclatureId = -1,
                order = "desc",
                sortedBy = "request_execution_date",
                status = "ALL"
            };
            /* var _searchModel = new SearchModel
             {
                 criteria1 = "",
                 criteria3 = "",
                 id3 = -1,
                 maxResult = 50,
                 nomenclatureId = -1,
                 order = "desc",
                 sortedBy = "request_execution_date",
                 status = "ALL"
             };*/
            var request = JsonConvert.SerializeObject(_searchModel);
            Debug.WriteLine("********request*************");
            Debug.WriteLine(request);
            var content = new StringContent(request, Encoding.UTF8, "application/json");

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/executionReport/exportExcel";
            Debug.WriteLine("********url*************");
            Debug.WriteLine(url);
            DependencyService.Get<INotification>().CreateNotification("Download Excel", "Please wait a few seconds !");
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                return;
            }
            PopuPage page1 = new PopuPage();
            await PopupNavigation.Instance.PushAsync(page1);
            await Task.Delay(2000);
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

                await DependencyService.Get<ISave>().SaveAndView("Request-execution-"+ dateNow + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", stream);
            }
        }
    }
}