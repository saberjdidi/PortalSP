using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public class UpdateRequestViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private bool value = false;
        private Request _requestObject;
        private Request _request;
       // private DateTime SamplingDate;
        #endregion

        #region Properties
        public int IdRequest { get; set; }
        public int IdPatient { get; set; }
        public int IdClient { get; set; }
        public int IdDoctorNoRef { get; set; }
        public Request RequestId { get; set; }
        public Request Request
        {
            get { return _requestObject; }
            set
            {
                _requestObject = value;
                OnPropertyChanged();
            }
        }
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

        #region Constructor
        public UpdateRequestViewModel()
        {
            apiService = new ApiServices();
            GetRequest();

            ListClientAutoComplete();
            ListRequestCatalogAutoComplete();
            ListBranchAutoComplete();
            ListNomenclaturaAutoComplete();
            ListIcdoAutoComplete();
            //SiapecListAutoComplete();
            ListUserAutoComplete();
            ListRoomAutoComplete();
            ListInstrumentAutoComplete();
            //ListDoctorAutoComplete();

        }
        #endregion

        #region Methods
        public async void GetRequest()
        {

            MessagingCenter.Subscribe<PassIdPatient>(this, "UpdateRequestId", async (value) =>
            {
                IdRequest = value.idPatient;
                Debug.WriteLine("********Id of request*************");
                Debug.WriteLine(IdRequest);

                var timestamp = DateTime.Now.ToFileTime();
                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/request/getById?id=" + IdRequest + "&time=" + timestamp;
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
                var list = JsonConvert.DeserializeObject<Request>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********list*************");
                Debug.WriteLine(list);
                Request = (Request)list;
            });
        }

        public async void EditRequest()
        {
           //DateTime SamplingDate = Convert.ToDateTime(Request.samplingDate);
           //string DateToString = SamplingDate.ToString("yyyy-MM-dd HH:mm:ss");
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
             if (Request.client == null)
             {
                 Value = true;
                 return;
             }
            var _biologicalMaterials = new List<BiologicalMaterials>()
                       {
                           new BiologicalMaterials(){barCode =  "", description= "external"}
                       };
            if(Request.requestCatalog == null)
            {
            _request = new Request
            {
                id = Request.id,
                patient = Request.patient,
                client = Request.client,
                status = Request.status,
                price = Request.price,
                branch = Request.branch,
                checkDate = Request.checkDate,
                code = Request.code,
                creationDate = Request.creationDate,
                executionDate = Request.executionDate,
                icdo = Request.icdo,
                instrument = Request.instrument,
                room = Request.room,
                nomenclatura = Request.nomenclatura,
                requestCatalog = Request.requestCatalog,
                siapec = Request.siapec,
                biologicalMaterials = _biologicalMaterials,
                groupId = Request.groupId,
                billDownloaded = Request.billDownloaded,
                isCloned = Request.isCloned,
                isHandled = Request.isHandled,
                isMaster = Request.isMaster,
                isPositive = Request.isPositive,
                samplingDate = Request.samplingDate,
                doctorNoRef = Request.doctorNoRef,
                drugDescription = Request.drugDescription,
                createBy = Request.createBy
            };
            }
            else
            {
                _request = new Request
                {
                    id = Request.id,
                    patient = Request.patient,
                    client = Request.client,
                    status = Request.status,
                    price = Request.price,
                    branch = Request.requestCatalog.branch,
                    checkDate = Request.checkDate,
                    code = Request.code,
                    creationDate = Request.creationDate,
                    executionDate = Request.executionDate,
                    icdo = Request.requestCatalog.icdo,
                    instrument = Request.instrument,
                    room = Request.room,
                    nomenclatura = Request.requestCatalog.nomenclatura,
                    requestCatalog = Request.requestCatalog,
                    siapec = Request.requestCatalog.siapec,
                    biologicalMaterials = _biologicalMaterials,
                    groupId = Request.groupId,
                    billDownloaded = Request.billDownloaded,
                    isCloned = Request.isCloned,
                    isHandled = Request.isHandled,
                    isMaster = Request.isMaster,
                    isPositive = Request.isPositive,
                    samplingDate = Request.samplingDate,
                    doctorNoRef = Request.doctorNoRef,
                    drugDescription = Request.drugDescription,
                    createBy = Request.createBy
                };
            }
            var requestUpdate = new RequestUpdate
            {
                request = _request,
                biologicalMaterials = _biologicalMaterials
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<RequestUpdate>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/request/update",
            res,
            requestUpdate);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            /*if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }*/

            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Request Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand UpdateRequest
        {
            get
            {
                return new Command(() =>
                {
                    EditRequest();
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
        //Client
        private List<Client> _clientAutoComplete;
        public List<Client> ClientAutoComplete
        {
            get { return _clientAutoComplete; }
            set
            {
                _clientAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Client>> ListClientAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                order = "asc",
                sortedBy = "companyName"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Client>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/client/searchSample",
            res,
            _searchModel);
            ClientAutoComplete = (List<Client>)response.Result;
            return ClientAutoComplete;
        }
        //Request catalog
        private List<Requestcatalog> _requestCatalogAutoComplete;
        public List<Requestcatalog> RequestCatalogAutoComplete
        {
            get { return _requestCatalogAutoComplete; }
            set
            {
                _requestCatalogAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Requestcatalog>> ListRequestCatalogAutoComplete()
        {
            MessagingCenter.Subscribe<PassIdPatient>(this, "UpdateClientId", async (value) =>
            {
                IdClient = value.idPatient;
                Debug.WriteLine("********Id of Client*************");
                Debug.WriteLine(IdClient);
            
                var _search = new SearchRequestCatalog
                {
                    fromReflex = false,
                    id1 = IdClient,
                    order = "asc",
                    sortedBy = "description"
                };
                var cookie = Settings.Cookie;  //.Split(11, 33)
                var res = cookie.Substring(11, 32);
                var response = await apiService.PostRequestCatalog<Requestcatalog>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/requestCatalog/getDefaultRequestConfiguration",
                res,
                _search);
                RequestCatalogAutoComplete = (List<Requestcatalog>)response.Result;
            });
            return RequestCatalogAutoComplete;
        }
        //User
        private List<User> _userAutoComplete;
        public List<User> UserAutoComplete
        {
            get { return _userAutoComplete; }
            set
            {
                _userAutoComplete = value;
                OnPropertyChanged();
            }
        }

        public async Task<List<User>> ListUserAutoComplete()
        {
            //String timeStamp = GetTimestamp(DateTime.Now);
            var timestamp = DateTime.Now.ToFileTime();
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<User>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/user/list?sortedBy=userName&order=asc&time=" + timestamp,
            res);
            UserAutoComplete = (List<User>)response.Result;
            return UserAutoComplete;
        }
        //Nomenclature
        private List<Nomenclatura> _nomenclaturaAutoComplete;
        public List<Nomenclatura> NomenclaturaAutoComplete
        {
            get { return _nomenclaturaAutoComplete; }
            set
            {
                _nomenclaturaAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Nomenclatura>> ListNomenclaturaAutoComplete()
        {
           /* MessagingCenter.Subscribe<PassIdPatient>(this, "UpdateClientId", async (value) =>
            {
                IdClient = value.idPatient;
                Debug.WriteLine("********Id of Client*************");
                Debug.WriteLine(IdClient);
            });*/
            var _searchModel = new SearchModel
            {
                criteria0 = Request.requestCatalog.branch.code,
                criteria2 = Request.requestCatalog.icdo.code,
                criteria3 = Request.requestCatalog.siapec.code,
                id1 = IdClient,
                id2 = -1,
                id3 = -1,
                nomenclatureId = -1,
                order = "asc",
                sortedBy = "descrEsameProf",
                status = "ALL"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Nomenclatura>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/nomenclatura/advancedSearch",
            res,
            _searchModel);
            NomenclaturaAutoComplete = (List<Nomenclatura>)response.Result;
            
            return NomenclaturaAutoComplete;
        }
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
            MessagingCenter.Subscribe<PassIdPatient>(this, "UpdateClientId", async (value) =>
            {
                IdClient = value.idPatient;
                Debug.WriteLine("********Id of Client*************");
                Debug.WriteLine(IdClient);
            
            var _searchModel = new SearchModel
            {
                criteria1 = "",
                id1 = IdClient,
                order = "asc",
                sortedBy = "name"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Branch>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/branch/getConfiguredBranches",
            res,
            _searchModel);
            BranchAutoComplete = (List<Branch>)response.Result;
            });
            return BranchAutoComplete;
        }
        //Icdo
        private List<Icdo> _icdoAutoComplete;
        public List<Icdo> IcdoAutoComplete
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
            MessagingCenter.Subscribe<PassIdPatient>(this, "UpdatePatientId", async (value) =>
            {
                IdPatient = value.idPatient;
                Debug.WriteLine("********Id of Patient*************");
                Debug.WriteLine(IdPatient);
            
                var _searchModel = new SearchModel
            {
                id1 = IdPatient,
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
            IcdoAutoComplete = (List<Icdo>)response.Result;
            });
            return IcdoAutoComplete;
            
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
                criteria2 = "",
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
        //Room
        private List<Room> _roomAutoComplete;
        public List<Room> RoomAutoComplete
        {
            get { return _roomAutoComplete; }
            set
            {
                _roomAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Room>> ListRoomAutoComplete()
        {
            var _searchModel = new SearchInstrument
            {
                criteria1 = "",
                order = "asc",
                sortedBy = "description",
                status = "active"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequestInstrument<Room>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/room/search",
            res,
            _searchModel);
            RoomAutoComplete = (List<Room>)response.Result;
            return RoomAutoComplete;
        }
        //Instrument
        private List<Instrument> _instrumentAutoComplete;
        public List<Instrument> InstrumentAutoComplete
        {
            get { return _instrumentAutoComplete; }
            set
            {
                _instrumentAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Instrument>> ListInstrumentAutoComplete()
        {
            var _searchModel = new SearchInstrument
            {
                criteria1 = "",
                order = "asc",
                sortedBy = "description",
                status = "active"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequestInstrument<Instrument>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/instrument/search",
            res,
            _searchModel);
            InstrumentAutoComplete = (List<Instrument>)response.Result;
            return InstrumentAutoComplete;
        }
        //Doctor
        private List<DoctorNoRef> _doctorComplete;
        public List<DoctorNoRef> DoctorAutoComplete
        {
            get { return _doctorComplete; }
            set
            {
                _doctorComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<DoctorNoRef>> ListDoctorAutoComplete()
        {
            /*  var _searchModel = new SearchModel
              {
                  id3 = Request.doctorNoRef.clientsToManage.Select(r => r.id).FirstOrDefault(),
                  order = "asc",
                  sortedBy = "firstName"
              };
              var cookie = Settings.Cookie;  //.Split(11, 33)
              var res = cookie.Substring(11, 32);
              var response = await apiService.PostRequest<DoctorNoRef>(
              "https://portalesp.smart-path.it",
              "/Portalesp",
              "/request/getUserByRole",
              res,
              _searchModel);
              DoctorAutoComplete = (List<DoctorNoRef>)response.Result;
              return DoctorAutoComplete;*/
            MessagingCenter.Subscribe<PassIdPatient>(this, "DoctorNoRefId", async (value) =>
            {
                IdDoctorNoRef = value.idPatient;
                Debug.WriteLine("********Id of DoctorRef*************");
                Debug.WriteLine(IdDoctorNoRef);
            
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var _search = new SearchDoctorNoRef
            {
                id3 = 179, //Request.doctorNoRef.clientsToManage.Select(r => r.id).FirstOrDefault(),
                order = "asc",
                sortedBy = "firstName"
            };

            var requestJson = JsonConvert.SerializeObject(_search);
            Debug.WriteLine("********request doctor*************");
            Debug.WriteLine(requestJson);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/request/getUserByRole";
            Debug.WriteLine("********url*************");
            Debug.WriteLine(url);
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            var response = await client.PostAsync(url, content);
            if (!response.IsSuccessStatusCode)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                
            }
            var result = await response.Content.ReadAsStringAsync();
            Debug.WriteLine("********result doctor*************");
            Debug.WriteLine(result);
            //var newRecord = JsonConvert.DeserializeObject<T>(result);
            var list = JsonConvert.DeserializeObject<List<DoctorNoRef>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            DoctorAutoComplete = (List<DoctorNoRef>)list;
            });
            return DoctorAutoComplete;
        }
        #endregion
    }
    #region Model
      public class SearchDoctorNoRef
    {
        public int id3 { get; set; }
        public string order { get; set; }
        public string sortedBy { get; set; }
    }
    #endregion
}
