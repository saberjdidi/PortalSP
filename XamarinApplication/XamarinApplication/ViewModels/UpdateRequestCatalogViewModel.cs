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
    public class UpdateRequestCatalogViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Requestcatalog _requestcatalog;
        #endregion

        #region Constructors
        public UpdateRequestCatalogViewModel()
        {
            apiService = new ApiServices();
            GetRequestCatalog();

            ListBranchAutoComplete();
            ListIcdoAutoComplete();
            ListNomenclaturaAutoComplete();
            SiapecListAutoComplete();
        }
        #endregion

        #region Properties
        public int IdRequestcatalog { get; set; }
        public Requestcatalog RequestcatalogId { get; set; }
        public Requestcatalog RequestCatalog
        {
            get { return _requestcatalog; }
            set
            {
                _requestcatalog = value;
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
        #endregion

        #region Methods
        public async void GetRequestCatalog()
        {

            MessagingCenter.Subscribe<PassIdPatient>(this, "UpdateRequestCatalogId", async (value) =>
            {
                IdRequestcatalog = value.idPatient;
                Debug.WriteLine("********IdRequestcatalog*************");
                Debug.WriteLine(IdRequestcatalog);

                var timestamp = DateTime.Now.ToFileTime();
                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/requestCatalog/getById?id=" + IdRequestcatalog + "&time=" + timestamp;
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
                Debug.WriteLine("********result requestCatalog*************");
                Debug.WriteLine(result);
                var list = JsonConvert.DeserializeObject<Requestcatalog>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********list requestCatalog*************");
                Debug.WriteLine(list);
                RequestCatalog = (Requestcatalog)list;
            });
        }
        public async void EditRequestCatalog()
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
            if (string.IsNullOrEmpty(RequestCatalog.code) || string.IsNullOrEmpty(RequestCatalog.description))
            {
                Value = true;
                return;
            }
            if (RequestCatalog.branch == null || RequestCatalog.icdo == null || RequestCatalog.nomenclatura == null)
            {
                Value = true;
                //await Application.Current.MainPage.DisplayAlert("Warning", "Branch is required", "ok");
                return;
            }
            var requestCatalog = new Requestcatalog
            {
                id = RequestCatalog.id,
                code = RequestCatalog.code,
                description = RequestCatalog.description,
                branch = RequestCatalog.branch,
                icdo = RequestCatalog.icdo,
                siapec = RequestCatalog.siapec,
                nomenclatura = RequestCatalog.nomenclatura,
                valid = RequestCatalog.valid
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<Requestcatalog>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/requestCatalog/update",
            res,
            requestCatalog);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            RequestCatalogViewModel.GetInstance().Update(requestCatalog);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "RequestCatalog Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand UpdateRequestCatatog
        {
            get
            {
                return new Command(() =>
                {
                    EditRequestCatalog();
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
        #endregion

        #region Autocomplete
        //Branch
        private List<Branch> _branchAutoComplete;
        public List<Branch> BranchAutoComplete
        {
            get { return _branchAutoComplete; }
            set
            {
                _branchAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Branch>> ListBranchAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                criteria1 = "",
                order = "asc",
                sortedBy = "name"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Branch>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/branch/search",
            res,
            _searchModel);
            BranchAutoComplete = (List<Branch>)response.Result;
            return BranchAutoComplete;
        }
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
        public async Task<List<Siapec>> SiapecListAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                criteria2 = RequestCatalog.icdo.code,
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
        //Procedure(Nomenclature)
        private List<Nomenclatura> _nomenclatureAutoComplete;
        public List<Nomenclatura> NomenclaturAutoComplete
        {
            get { return _nomenclatureAutoComplete; }
            set
            {
                _nomenclatureAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Nomenclatura>> ListNomenclaturaAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                criteria0 = "",
                criteria2 = "",
                criteria3 = "",
                order = "asc",
                sortedBy = "descrEsameProf"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Nomenclatura>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/nomenclatura/advancedSearch",
            res,
            _searchModel);
            NomenclaturAutoComplete = (List<Nomenclatura>)response.Result;
            return NomenclaturAutoComplete;
        }
        #endregion
    }
}
