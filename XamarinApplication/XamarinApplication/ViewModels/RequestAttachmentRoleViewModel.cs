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
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class RequestAttachmentRoleViewModel : BaseViewModel
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
        public RequestAttachmentRoleViewModel()
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

            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetAttachmentWithCoockie<AttachmentListRestrict>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/request/getAttachmentListRestrict?requestId=" + Attachment.requests.Select(r => r.id).FirstOrDefault() + "&clientId=" + Attachment.requests.Select(r => r.client.id).FirstOrDefault(),
                 res);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Attachments = (AttachmentListRestrict)response.Result;
            if (Attachments.attachments.Count() == 0)
            {
                IsVisibleStatus = true;
            }
            else
            {
                IsVisibleStatus = false;
            }
        }
        #endregion

        #region Sigleton
        static RequestAttachmentRoleViewModel instance;
        public static RequestAttachmentRoleViewModel GetInstance()
        {
            if (instance == null)
            {
                return new RequestAttachmentRoleViewModel();
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
            var url = "https://portalesp.smart-path.it/Portalesp/attachment/downloadAttachmentFile?id=" + attachment.id;
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
