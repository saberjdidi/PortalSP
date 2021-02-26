using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
   public class ConsentDocumentPatientTrueViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<ConsentDocumentTrue> _document;
        private List<ConsentDocumentTrue> consentDocumentList;
        private bool isVisible;
        private bool isRefreshing;
        #endregion

        #region Properties
        public Patient Patient { get; set; }
        public ObservableCollection<ConsentDocumentTrue> ConsentDocument
        {
            get { return _document; }
            set
            {
                _document = value;
                OnPropertyChanged();
            }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
                OnPropertyChanged();
            }
        }
        public bool IsRefreshing
        {
            get
            {
                return isRefreshing;
            }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public ConsentDocumentPatientTrueViewModel()
        {
            apiService = new ApiServices();
            GetConsentDocument();
            instance = this;
        }
        #endregion

        #region Methods
        public async void GetConsentDocument()
        {
            IsRefreshing = true;
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
            var response = await apiService.GetListWithCoockie<ConsentDocumentTrue>(
             "https://portalesp.smart-path.it",
             "/Portalesp",
             "/repository/searchSavedDocument?sortedBy=code&order=asc&patientId=" + Patient.id,
             res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            consentDocumentList = (List<ConsentDocumentTrue>)response.Result;
            ConsentDocument = new ObservableCollection<ConsentDocumentTrue>(consentDocumentList);
            IsRefreshing = false;
            if (ConsentDocument.Count() == 0)
            {
                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }

        }
        #endregion

        #region Sigleton
        static ConsentDocumentPatientTrueViewModel instance;
        public static ConsentDocumentPatientTrueViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ConsentDocumentPatientTrueViewModel();
            }

            return instance;
        }
        public async Task Download(ConsentDocumentTrue consentDocument)
        {
            IsRefreshing = true;
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }

            var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            //get Document
            var getUrl = "https://portalesp.smart-path.it/Portalesp/patient/getHeaderDocument?documentId=" + consentDocument.document.repositoryTemplate.id + "&patientId=" + Patient.id;
            Debug.WriteLine("+++++++++++++++++++++++++getUrl++++++++++++++++++++++++");
            Debug.WriteLine(getUrl);
            client.BaseAddress = new Uri(getUrl);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var getResponse = await client.GetAsync(getUrl);
            if (!getResponse.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Error", getResponse.StatusCode.ToString(), "ok");
                return;
            }
            var getResult = await getResponse.Content.ReadAsStringAsync();
            Debug.WriteLine("+++++++++++++++++++++++++getResult++++++++++++++++++++++++");
            Debug.WriteLine(getResult);
            var getDocument = JsonConvert.DeserializeObject<DocumentConsentPatient>(getResult, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            //download word
            var _model = new DocumentConsentPatient
            {
                documentCode = getDocument.documentCode,
                documentVersion = getDocument.documentVersion,
                documentProperties = getDocument.documentProperties
            };
            var request = JsonConvert.SerializeObject(_model);
            Debug.WriteLine("********request*************");
            Debug.WriteLine(request);
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            var url = "https://portalesp.smart-path.it/port-docs/smartmaker/document/fill/base64?code=" + getDocument.documentCode + "&version=" + getDocument.documentVersion;
            Debug.WriteLine("********url*************");
            Debug.WriteLine(url);
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var response = await client.PostAsync(url, content);
            Debug.WriteLine("********response*************");
            Debug.WriteLine(response);
            if (!response.IsSuccessStatusCode)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                return;
            }

            var result = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("********result*************");
            Debug.WriteLine(result);
            var word = JsonConvert.DeserializeObject<DownloadWordDocument>(result);
            IsRefreshing = false;
            byte[] bytes = Convert.FromBase64String(word.content);
            MemoryStream stream = new MemoryStream(bytes);

            if (stream == null)
            {
                await Application.Current.MainPage.DisplayAlert("Warning", "Data is Empty", "ok");
                return;
            }
            await DependencyService.Get<ISave>().SaveAndView(getDocument.documentCode + "-" + dateNow + ".DOCX", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", stream);
        }
        #endregion
    }
}
