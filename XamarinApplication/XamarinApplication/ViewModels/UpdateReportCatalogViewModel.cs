using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
   public class UpdateReportCatalogViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private ReportCatalog _reportCatalog;
        #endregion

        #region Constructors
        public UpdateReportCatalogViewModel()
        {
            apiService = new ApiServices();
            GetReport();

            ListIcdoAutoComplete();
            ListRagServiceAutoComplete();
            //ListSiapecAutoComplete();
        }
        #endregion

        #region Properties
        public int IdReport { get; set; }
        public ReportCatalog ReportCatalogId { get; set; }
        public ReportCatalog ReportCatalog
        {
            get { return _reportCatalog; }
            set
            {
                _reportCatalog = value;
                OnPropertyChanged();
            }
        }
        private bool value = false;
        public bool Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }
        private bool _showIntervention = true;
        public bool ShowIntervention
        {
            get => _showIntervention;
            set
            {
                _showIntervention = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void GetReport()
        {

            MessagingCenter.Subscribe<PassIdPatient>(this, "UpdateReportId", async (value) =>
            {
                IdReport = value.idPatient;
                Debug.WriteLine("********Id of user*************");
                Debug.WriteLine(IdReport);

                var timestamp = DateTime.Now.ToFileTime();
                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/reportCatalog/getById?id=" + IdReport + "&time=" + timestamp;
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
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                var list = JsonConvert.DeserializeObject<ReportCatalog>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********list*************");
                Debug.WriteLine(list);
                ReportCatalog = (ReportCatalog)list;
            });
        }
        public async void EditReport()
        {
            Value = true;
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            var report = new ReportCatalog
            {
                id = ReportCatalog.id,
                icdo = ReportCatalog.icdo,
                ragService = ReportCatalog.ragService,
                siapecs = ReportCatalog.siapecs
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<ReportCatalog>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/reportCatalog/update",
            res,
            report);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            ReportCatalogViewModel.GetInstance().Update(report);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Report Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Update
        {
            get
            {
                return new Command(() =>
                {
                    EditReport();
                });
            }
        }
        public Command ClosePopup
        {
            get
            {
                return new Command(() =>
                {
                    Navigation.PopPopupAsync();
                    //Navigation.PopAsync();
                    Debug.WriteLine("********Close*************");
                });
            }
        }
        public ICommand OpenTopograficCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (ReportCatalog.icdo == null)
                    {
                        Application.Current.MainPage.DisplayAlert("Alert", "Please Select Topografic", "ok");
                        return;
                    }
                    else
                    {
                        ListSiapecAutoComplete();
                        ShowIntervention = true;
                    }
                });
            }
        }
        #endregion

        #region Autocomplete
        //ICDO
        private List<Icdo> _icdoAutoComplete;
        public List<Icdo> ICDOAutoComplete
        {
            get { return _icdoAutoComplete; }
            set
            {
                _icdoAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Icdo>> ListIcdoAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                order = "asc",
                sortedBy = "description"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Icdo>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/icdo/search",
            res,
            _searchModel);
            ICDOAutoComplete = (List<Icdo>)response.Result;
            return ICDOAutoComplete;
        }
        //RagService
        private List<RagService> _ragServiceAutoComplete;
        public List<RagService> RagServiceAutoComplete
        {
            get { return _ragServiceAutoComplete; }
            set
            {
                _ragServiceAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<RagService>> ListRagServiceAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                order = "asc",
                sortedBy = "description"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<RagService>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/ragService/search",
            res,
            _searchModel);
            RagServiceAutoComplete = (List<RagService>)response.Result;
            return RagServiceAutoComplete;
        }
        //Intervention(siapec)
        private List<Siapec> _siapecAutoComplete;
        public List<Siapec> SIAPECAutoComplete
        {
            get { return _siapecAutoComplete; }
            set
            {
                _siapecAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Siapec>> ListSiapecAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                criteria2 = ReportCatalog.icdo.code,
                order = "asc",
                sortedBy = "description"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Siapec>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/nomenclatura/getNomenclaturaSiapecs/search",
            res,
            _searchModel);
            SIAPECAutoComplete = (List<Siapec>)response.Result;
            return SIAPECAutoComplete;
        }
        #endregion
    }
}
