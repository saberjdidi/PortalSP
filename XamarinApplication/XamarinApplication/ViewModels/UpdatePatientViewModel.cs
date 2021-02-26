using Newtonsoft.Json;
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
    public class UpdatePatientViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Patient _patient;
        #endregion

        #region Constructors
        public UpdatePatientViewModel(INavigation _navigation)
        {
            Navigation = _navigation;
            apiService = new ApiServices();
            GetPatient();

            ListDomicileAutoComplete();
            ListComuniLocalAutoComplete();
            ListClientAutoComplete();
            //ListGenderAutoComplete();
            ListTitleAutoComplete();

            ListTitle = GetTitle().ToList();
            ListSex = GetSex().OrderBy(t => t.Value).ToList();
            ListConditionPayment = GetConditionPayment().ToList();
        }
        #endregion

        #region Properties
        public Patient PatientId { get; set; }
        public Patient Patient
        {
            get { return _patient; }
            set
            {
                _patient = value;
                OnPropertyChanged();
            }
        }
        public int IdPatient { get; set; }
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
        public async void GetPatient()
        {

            MessagingCenter.Subscribe<PassIdPatient>(this, "UpdatePatientId", async (value) =>
            {
                IdPatient = value.idPatient;
                Debug.WriteLine("********Id of patient*************");
                Debug.WriteLine(IdPatient);

                var timestamp = DateTime.Now.ToFileTime();
                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/patient/getById?id=" + IdPatient + "&time=" + timestamp;
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
                var list = JsonConvert.DeserializeObject<Patient>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Patient = (Patient)list;
            });
        }
        public async void EditPatient()
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
           if (string.IsNullOrEmpty(Patient.firstName) || string.IsNullOrEmpty(Patient.lastName) || string.IsNullOrEmpty(Patient.fiscalCode))
            {
                Value = true;
                return;
            }
            if (Patient.client == null || Patient.residence == null || Patient.domicile == null)
            {
                Value = true;
                return;
            }
            if (SelectedTitle.Key == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please Select Title", "ok");
                return;
            }
            if (SelectedSex.Key == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please Select Gender", "ok");
                return;
            }
             var _gender = new Gender
             {
                 enumType = "eu.smartpath.portalesp.enums.Gender",
                 name = SelectedSex.Key
             };
            var _fiscalData = new FiscalData
             {
                 id = Patient.fiscalData.id,
                 codeDestinaterio = Patient.fiscalData.codeDestinaterio,
                 inderizioPec = Patient.fiscalData.inderizioPec,
                 adresseFacturation = Patient.fiscalData.adresseFacturation,
                 conditionPaymentDescription = Patient.fiscalData.conditionPaymentDescription,
                 pIVA = Patient.fiscalData.pIVA
             };
            var _domicile = new Domicile
            {
                id = Patient.domicile.id,
                comuniLocal = Patient.domicile.comuniLocal,
                street = Patient.domicile.street
            };
            var _residence = new Residence
            {
                id = Patient.residence.id,
                comuniLocal = Patient.residence.comuniLocal,
                street = Patient.residence.street
            };
            var patient = new Patient
            {
                id = Patient.id,
                title = Patient.title, // SelectedTitle.Key,
                firstName = Patient.firstName,
                lastName = Patient.lastName,
                fullName = Patient.fullName,
                fiscalCode = Patient.fiscalCode,
                gender =  _gender, //Patient.gender,
                birthDate = Patient.birthDate,
                placeOfBirth = Patient.placeOfBirth,
                client = Patient.client,
                phone = Patient.phone,
                cellPhone = Patient.cellPhone,
                email = Patient.email,
                note = Patient.note,
                fiscalData = _fiscalData,
                domicile = _domicile,
                residence = _residence,
                confirmSave = false,
                isMerged = false,
                isRepositorySaved = false
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.PutPatient<Patient>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/patient/update",
            res,
            patient);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            /* if (!response.IsSuccess)
             {
                 await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                 return;
             }*/
            Value = false;
            PatientViewModel.GetInstance().Update(patient);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Patient Updated");
            await Navigation.PopAsync();
        }
        #endregion

        #region Commands
        public ICommand Update
        {
            get
            {
                return new Command(() =>
                {
                    EditPatient();
                    Debug.WriteLine("********update*************");
                });
            }
        }
        public Command ClosePopup
        {
            get
            {
                return new Command(() =>
                {
                    //Navigation.PopPopupAsync();
                    Navigation.PopAsync();
                    Debug.WriteLine("********Close*************");
                });
            }
        }
        #endregion

        #region Autocomplete
        //Domicile
        private List<ComuniLocal> _domicileAutoComplete;
        public List<ComuniLocal> DomicileAutoComplete
        {
            get { return _domicileAutoComplete; }
            set
            {
                _domicileAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<ComuniLocal>> ListDomicileAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                order = "asc",
                sortedBy = "description"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<ComuniLocal>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/comuniLocal/search",
            res,
            _searchModel);
            DomicileAutoComplete = (List<ComuniLocal>)response.Result;
            return DomicileAutoComplete;
        }
        //Residence
        private List<ComuniLocal> _comuniLocal;
        public List<ComuniLocal> ComuniLocalAutoComplete
        {
            get { return _comuniLocal; }
            set
            {
                _comuniLocal = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<ComuniLocal>> ListComuniLocalAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                order = "asc",
                sortedBy = "description"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<ComuniLocal>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/comuniLocal/search",
            res,
            _searchModel);
            ComuniLocalAutoComplete = (List<ComuniLocal>)response.Result;
            return ComuniLocalAutoComplete;
        }
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
        //title
        public List<string> StringAutoComplete { get; set; }
        public void ListTitleAutoComplete()
        {
            var titles = new List<string>() { "Sig.ra", "Sig.na", "Sig.", "Dott", "Dott.ssa", "Spett.", "Prof." };
            StringAutoComplete = new List<string>(titles);
        }
        //Gender
        private List<Gender> _genderAutoComplete;
        public List<Gender> GenderAutoComplete
        {
            get { return _genderAutoComplete; }
            set
            {
                _genderAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public void ListGenderAutoComplete()
        {
            var gender = new List<Gender>
            {
                new Gender { name="M", enumType="eu.smartpath.portalesp.enums.Gender"},
                new Gender { name="F", enumType="eu.smartpath.portalesp.enums.Gender"}
            };
            GenderAutoComplete = new List<Gender>(gender);
        }
        #endregion

        #region Status
        //*********Title**************
        public List<Language> ListTitle { get; set; }
        private Language _selectedTitle { get; set; }
        public Language SelectedTitle
        {
            get { return _selectedTitle; }
            set
            {
                if (_selectedTitle != value)
                {
                    _selectedTitle = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<Language> GetTitle()
        {
            var languages = new List<Language>()
            {
                new Language(){Key =  "Sig.ra", Value= "Sig.ra"},
                new Language(){Key =  "Sig.na", Value= "Sig.na"},
                new Language(){Key =  "Sig.", Value= "Sig."},
                new Language(){Key =  "Dott", Value= "Dott"},
                new Language(){Key =  "Dott.ssa", Value= "Dott.ssa"},
                new Language(){Key =  "Spett.", Value= "Spett."},
                new Language(){Key =  "Prof.", Value= "Prof."}
            };

            return languages;
        }
        //***********Sex**********
        public List<Language> ListSex { get; set; }
        private Language _selectedSex { get; set; }
        public Language SelectedSex
        {
            get { return _selectedSex; }
            set
            {
                if (_selectedSex != value)
                {
                    _selectedSex = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<Language> GetSex()
        {
            var languages = new List<Language>()
            {
                new Language(){Key =  "M", Value= "Male"},
                new Language(){Key =  "F", Value= "Female"}
            };

            return languages;
        }
        //*********Condition Payment**************
        public List<Language> ListConditionPayment { get; set; }
        public Language SelectedConditionPayment { get; set; }
        public List<Language> GetConditionPayment()
        {
            var languages = new List<Language>()
            {
                new Language(){Key =  "Cash", Value= "Cash"},
                new Language(){Key =  "Credit/Debit Card", Value= "Credit/Debit Card"},
                new Language(){Key =  "Bank Transfer", Value= "Bank Transfer"}
            };

            return languages;
        }
        #endregion
    }
}
