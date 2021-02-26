using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
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
    public class NewRequestViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private bool value = false;
        private AddRequest _addRequest;
        private SearchRequestCatalog _search;
        private SearchModel _searchModel;
        private string _price = "0";
        #endregion

        #region Properties
        public Patient Patient { get; set; }
        public int IdClient { get; set; }
        public int IdPatient { get; set; }
        public string CustomerPrice
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }
        private Client _client = null;
        public Client Client
        {
            get { return _client; }
            set
            {
                _client = value;
                OnPropertyChanged();
            }
        }
        private Requestcatalog _requestCatalog = null;
        public Requestcatalog RequestCatalog
        {
            get { return _requestCatalog; }
            set
            {
                _requestCatalog = value;
                OnPropertyChanged();
            }
        }
        private User _user = null;
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }
        private Room _room = null;
        public Room Room
        {
            get { return _room; }
            set
            {
                _room = value;
                OnPropertyChanged();
            }
        }
        private Instrument _instrument = null;
        public Instrument Instrument
        {
            get { return _instrument; }
            set
            {
                _instrument = value;
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
        private bool _showHide = false;
        public bool ShowHide
        {
            get => _showHide;
            set
            {
                _showHide = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructor
        public NewRequestViewModel()
        {
            apiService = new ApiServices();

            ListClientAutoComplete();
            //ListRequestCatalogAutoComplete();
            GetRequestCatalog();
            //ListUserAutoComplete();
            ListRoomAutoComplete();
            ListInstrumentAutoComplete();

            MessagingCenter.Subscribe<PassIdPatient>(this, "ClientId", async (value) =>
            {
                IdClient = value.idPatient;
                Debug.WriteLine("********Id of Client*************");
                Debug.WriteLine(IdClient);
            });
            MessagingCenter.Subscribe<PassIdPatient>(this, "PatientId", async (value) =>
            {
                IdPatient = value.idPatient;
                Debug.WriteLine("********Id of Patient*************");
                Debug.WriteLine(IdPatient);
            });
        }
        #endregion

        #region Methods
        public async void GetRequestCatalog()
        {
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            if (Patient.client == null)
            {
                _search = new SearchRequestCatalog
                {
                    fromReflex = false,
                    id1 = 147,
                    id2 = -1,
                    order = "asc",
                    sortedBy = "description"
                };
            } else
            {
                _search = new SearchRequestCatalog
                {
                    fromReflex = false,
                    id1 = IdClient,
                    id2 = -1,
                    order = "asc",
                    sortedBy = "description"
                };
            }
            
            
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
           /* var response = await apiService.PostRequestCatalog<Requestcatalog>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/requestCatalog/getDefaultRequestConfiguration",
            res,
            _search);
            RequestCatalogAutoComplete = (List<Requestcatalog>)response.Result;*/
                var requestJson = JsonConvert.SerializeObject(_search);
                Debug.WriteLine("********request catalog*************");
                Debug.WriteLine(requestJson);
                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/requestCatalog/getDefaultRequestConfiguration";
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
                Debug.WriteLine("********result catalog*************");
                Debug.WriteLine(result);
                var list = JsonConvert.DeserializeObject<List<Requestcatalog>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                RequestCatalogAutoComplete = (List<Requestcatalog>)list;

            
        }
        public async void GetPrice()
        {
            if(Patient.client == null || RequestCatalog == null)
            {
                CustomerPrice = "0";
                return;
            }
            else
            {
                var cookie = Settings.Cookie;  //.Split(11, 33)
                var res = cookie.Substring(11, 32);
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/request/getConfiguredPrice?clientId=" + Patient.client.id + "&nomenclatureId=" + RequestCatalog.nomenclatura.id;
                Debug.WriteLine("********url price*************");
                Debug.WriteLine(url);
                client.BaseAddress = new Uri(url);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");

                }
                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result price*************");
                Debug.WriteLine(result);
               // var list = JsonConvert.DeserializeObject<double>(result);
                 
                  if (result.Equals("null"))
                   {
                      CustomerPrice = "4";
                       Debug.WriteLine("********Price*************");
                       Debug.WriteLine(CustomerPrice);
                   }
                   else
                   {
                    //CustomerPrice = Convert.ToDouble(list); //float.Parse(result);
                    CustomerPrice = result;
                    Debug.WriteLine("********CustomerPrice*************");
                    Debug.WriteLine(CustomerPrice);
                    // NumberFormatInfo provider = new NumberFormatInfo();
                    // provider.NumberDecimalSeparator = ".";
                    // provider.NumberGroupSeparator = ",";
                    //CustomerPrice = Convert.ToDouble(result, provider);
                    //CustomerPrice = System.Convert.ToDecimal(result);
                    //CustomerPrice = decimal.Parse(result, CultureInfo.InvariantCulture);
                   /* try
                    {
                    }
                    catch (Exception ex)
                    {
                       await Application.Current.MainPage.DisplayAlert("Alert", ex.Message, "ok");
                        return;
                    }*/
                }
            }
        }
        public async void AddRequest()
        {
            Debug.WriteLine("********Save*************");
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
            if (Patient.client == null)
            {
                Value = true;
                return;
            }
            if (RequestCatalog == null)
            {
                // await Application.Current.MainPage.DisplayAlert("Alert", "Please Select Request Catalog", "ok");
                // return;
                _addRequest = new AddRequest
                {
                    patient = Patient,
                    client = Patient.client,
                    status = "TC",
                    price = "0"
                    //requestCatalog = RequestCatalog,
                    //branch = RequestCatalog.branch,
                    //icdo = RequestCatalog.icdo,
                    //nomenclatura = RequestCatalog.nomenclatura,
                    //siapec = RequestCatalog.siapec,
                    //room = Room,
                    //instrument = Instrument
                };
            } else
            {
                _addRequest = new AddRequest
                {
                    patient = Patient,
                    client = Patient.client,
                    requestCatalog = RequestCatalog,
                    branch = RequestCatalog.branch,
                    icdo = RequestCatalog.icdo,
                    nomenclatura = RequestCatalog.nomenclatura,
                    siapec = RequestCatalog.siapec,
                    status = "TC",
                    price = CustomerPrice,
                    room = Room,
                    instrument = Instrument
                };
            }
            var _biologicalMaterials = new List<BiologicalMaterials>()
                       {
                           new BiologicalMaterials(){barCode =  "", description= "external"}
                       };
        var request = new RequestJson
             {
                 isCloned = false,
                 request = _addRequest,
                 biologicalMaterials = _biologicalMaterials
             };
             var cookie = Settings.Cookie;  //.Split(11, 33)
             var res = cookie.Substring(11, 32);

             var response = await apiService.Save<RequestJson>(
             "https://portalesp.smart-path.it",
             "/Portalesp",
             "/request/save",
             res,
             request);
             Debug.WriteLine("********responseIn ViewModel*************");
             Debug.WriteLine(response);
             /*if (!response.IsSuccess)
             {
                 await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                 return;
             }*/
             
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Request Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand SaveRequest
        {
            get
            {
                return new Command(() =>
                {
                    AddRequest();
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
        public ICommand OpenRequestCatalogCommand
        {
            get
            {
                return new Command(() =>
                {
                    /* if (RequestCatalog == null)
                     {
                         Application.Current.MainPage.DisplayAlert("Alert", "Please Select Request Catalog", "ok");
                         return;
                     }
                     else
                     {
                         ListBranchAutoComplete();
                         ListNomenclaturaAutoComplete();
                         ShowHide = true;
                     }*/
                    ListBranchAutoComplete();
                    ListNomenclaturaAutoComplete();
                    ListIcdoAutoComplete();
                    SiapecListAutoComplete();
                    GetPrice();
                    ShowHide = true;
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
            MessagingCenter.Subscribe<PassIdPatient>(this, "ClientId", async (value) =>
            {
                IdClient = value.idPatient;
                Debug.WriteLine("********Id of Client*************");
                Debug.WriteLine(IdClient);
                var _search = new SearchRequestCatalog
                {
                    fromReflex = false,
                    id1 = IdClient,
                    id2 = -1,
                    order = "asc",
                    sortedBy = "description"
                };
            
                var cookie = Settings.Cookie;  //.Split(11, 33)
                var res = cookie.Substring(11, 32);
                var requestJson = JsonConvert.SerializeObject(_search);
                Debug.WriteLine("********request catalog*************");
                Debug.WriteLine(requestJson);
                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/requestCatalog/getDefaultRequestConfiguration";
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
                Debug.WriteLine("********result catalog*************");
                Debug.WriteLine(result);
                //var newRecord = JsonConvert.DeserializeObject<T>(result);
                var list = JsonConvert.DeserializeObject<List<Requestcatalog>>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                RequestCatalogAutoComplete = (List<Requestcatalog>)list;
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
            "/user/list?sortedBy=userName&order=asc&time="+ timestamp,
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
            
            if (Patient.client == null && RequestCatalog == null)
            {
                _searchModel = new SearchModel
                {
                    criteria0 = "",
                    criteria2 = "",
                    criteria3 = "",
                    id1 = 166,
                    id2 = -1,
                    id3 = -1,
                    nomenclatureId = -1,
                    order = "asc",
                    sortedBy = "descrEsameProf",
                    status = "ALL"
                };
            } else if(RequestCatalog == null)
            {
                _searchModel = new SearchModel
                {
                    criteria0 = "",
                    criteria2 = "",
                    criteria3 = "",
                    id1 = IdClient,
                    id2 = -1,
                    id3 = -1,
                    nomenclatureId = -1,
                    order = "asc",
                    sortedBy = "descrEsameProf",
                    status = "ALL"
                };
            } else if (Patient.client == null)
            {
                _searchModel = new SearchModel
                {
                    criteria0 = RequestCatalog.branch.code,
                    criteria2 = RequestCatalog.icdo.code,
                    criteria3 = RequestCatalog.siapec.code,
                    id1 = 166,
                    id2 = -1,
                    id3 = -1,
                    nomenclatureId = -1,
                    order = "asc",
                    sortedBy = "descrEsameProf",
                    status = "ALL"
                };
            }
            else
            {
                _searchModel = new SearchModel
                {
                    criteria0 = RequestCatalog.branch.code,
                    criteria2 = RequestCatalog.icdo.code,
                    criteria3 = RequestCatalog.siapec.code,
                    id1 = IdClient,
                    id2 = -1,
                    id3 = -1,
                    nomenclatureId = -1,
                    order = "asc",
                    sortedBy = "descrEsameProf",
                    status = "ALL"
                };
            }
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
            if (Patient.client == null) {
                _searchModel = new SearchModel
                {
                    criteria1 = "",
                    id1 = 166,
                    order = "asc",
                    sortedBy = "name"
                };
            } else {
                _searchModel = new SearchModel
                {
                    criteria1 = "",
                    id1 = IdClient,
                    order = "asc",
                    sortedBy = "name"
                };
            }
             
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Branch>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/branch/getConfiguredBranches",
            res,
            _searchModel);
            BranchAutoComplete = (List<Branch>)response.Result;
            return BranchAutoComplete;
        }
        //Icdo (Topografic)
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
            if (RequestCatalog == null)
            {
                _searchModel = new SearchModel
                {
                    criteria2 = "",
                    id1 = IdPatient,
                    id2 = -1,
                    id3 = -1,
                    order = "asc",
                    sortedBy = "description"
                };
            }
            else
            {
                _searchModel = new SearchModel
                {
                    criteria2 = RequestCatalog.icdo.code,
                    id1 = IdPatient,
                    id2 = -1,
                    id3 = -1,
                    order = "asc",
                    sortedBy = "description"
                };
            }
                
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
        #endregion
    }
}
