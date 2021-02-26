using Newtonsoft.Json;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using System.IO;
using System.Threading.Tasks;

namespace XamarinApplication.ViewModels
{
    public class AttachmentListRestrictViewModel : BaseViewModel
    {
        
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        public Attachment Attachment { get; set; }
        private AttachmentListRestrict _attachmentListRestrict;
        bool _isVisibleStatus;
        #endregion

        #region Properties
        public AttachmentListRestrict Attachments
        {
            get { return _attachmentListRestrict; }
            set
            {
                _attachmentListRestrict = value;
                OnPropertyChanged();
            }
        }
        public bool IsVisibleStatus
        {
            get { return _isVisibleStatus; }
            set
            {
                _isVisibleStatus = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public AttachmentListRestrictViewModel()
        {
            apiService = new ApiServices();
            GetAttachmentRestrict();
            instance = this;
        }
        #endregion

        #region Methods
        public async void GetAttachmentRestrict()
        {
            var connection = await apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }
            /* var cookie = Settings.Cookie;  //.Split(11, 33)
             var res = cookie.Substring(11, 32);

             var cookieContainer = new CookieContainer();
             var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
             var client = new HttpClient(handler);
             cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
             var uri = "https://portalesp.smart-path.it/Portalesp/request/getAttachmentListRestrict?requestId=50958&clientId=1";
             //var response = await client.GetAsync(uri);
             //var result = await response.Content.ReadAsStringAsync();
             var result = await client.GetStringAsync(uri);
             AttachmentLists = JsonConvert.DeserializeObject<AttachmentList>(result);*/

            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetAttachmentWithCoockie<AttachmentListRestrict>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/request/getAttachmentListRestrict?requestId="+ Attachment.requests.Select(i => i.id).FirstOrDefault() + "&clientId=" + Attachment.requests.Select(i => i.client.id).FirstOrDefault(), //+ Attachment.requests.Select(i=> i.id ),
                 res);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Attachments = (AttachmentListRestrict)response.Result;
        }
        #endregion

        #region Sigleton
        static AttachmentListRestrictViewModel instance;
        public static AttachmentListRestrictViewModel GetInstance()
        {
            if (instance == null)
            {
                return new AttachmentListRestrictViewModel();
            }

            return instance;
        }
        public async Task Download(AttachmentRestrict attachment)
        {

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }

            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/attachment/downloadAttachmentFile?id="+ attachment.id;
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

                await DependencyService.Get<ISave>().SaveAndView(attachment.name + ".pdf", "application/pdf", stream);
            }
        }
        #endregion
    }
}
