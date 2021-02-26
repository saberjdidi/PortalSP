using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
    public class NewPatientViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewPatientViewModel(INavigation _navigation)
        {
            Navigation = _navigation;
            apiService = new ApiServices();

            ListComuniLocalAutoComplete();
            ListClientAutoComplete();
           // ListDomicileAutoComplete();

            ListTitle = GetTitle().ToList();
            ListSex = GetSex().OrderBy(t => t.Value).ToList();
           // ListConditionPayment = GetConditionPayment().ToList();
        }
        #endregion

        #region Properties
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
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FiscalCode { get; set; }
        public string Note { get; set; }
        public string Phone { get; set; }
        public string CellPhone { get; set; }
        public string Email { get; set; }
        public string CodeDestinatario { get; set; }
        public string InderizioPec { get; set; }
        public string PIVA { get; set; }
        public string BillingAddress { get; set; }
        public string NotePayment { get; set; }
        public string StreetDomicile { get; set; }
        public string StreetResidence { get; set; }
        private DateTime _birthDate = System.DateTime.Now;  //DateTime.Today.Date;
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set
            {
                _birthDate = value;
                OnPropertyChanged();
            }
        }
       // DateTime BirthDate = DateTime.ParseExact(DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
        private string dateNow = DateTime.Now.ToString("dd-MM-yyyy");
        public string DateNow
        {
            get { return dateNow; }
            set
            {
                dateNow = value;
                OnPropertyChanged();
            }
        }
        private ComuniLocal _placeOfBirth = null;
        public ComuniLocal PlaceOfBirth
        {
            get { return _placeOfBirth; }
            set
            {
                _placeOfBirth = value;
                OnPropertyChanged();
            }
        }
        private ComuniLocal _domicile = null;
        public ComuniLocal Domicile 
        {
            get { return _domicile; }
            set
            {
                _domicile = value;
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
        private ComuniLocal _residence = null;
        public ComuniLocal Residence
        {
            get { return _residence; }
            set
            {
                _residence = value;
                OnPropertyChanged();
            }
        }
        public decimal Length { get; set; }
        #endregion

        #region Methods
        public async void AddNewPatient()
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
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(FiscalCode))
            {
                Value = true;
                return;
            }
            if (Client == null || Residence == null || Domicile == null)
            {
                Value = true;
                return;
            }
            if(SelectedTitle.Key == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please Select Title", "ok");
                return;
            }
            if (SelectedSex.Key == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please Select Gender", "ok");
                return;
            }

            var _fiscalData = new FiscalData
            {
                codeDestinaterio = CodeDestinatario,
                inderizioPec = InderizioPec,
                adresseFacturation = BillingAddress,
                conditionPaymentDescription = NotePayment,
                conditionPayment = "CCD",
                pIVA = PIVA
            };
            var _domicile = new AddDomicile
            {
                comuniLocal = Domicile,
                street = StreetDomicile
            };
            var _residence = new AddResidence
            {
                comuniLocal = Residence,
                street = StreetResidence
            };
            var addPatient = new AddPatient
            {
                title = SelectedTitle.Key,
                firstName = FirstName,
                lastName = LastName,
                fullName = SelectedTitle.Key+" "+ FirstName+" "+ LastName,
                fiscalCode = FiscalCode,
                gender = SelectedSex.Key,
                birthDate = DateNow, //Convert.ToDateTime(DateNow),
                placeOfBirth = PlaceOfBirth,
                note = Note,
                phone = Phone,
                cellPhone = CellPhone,
                email =Email,
                //fiscalData = _fiscalData,
                client = Client,
                domicile = _domicile,
                residence = _residence
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddPatient>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/patient/save",
            res,
            addPatient);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
           /* if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }*/
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Patient Added");
            await Navigation.PopAsync();
        }
        #endregion

        #region Commands
        public ICommand SavePatient
        {
            get
            {
                return new Command(() =>
                {
                    AddNewPatient();
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
       /* private List<ComuniLocal> _domicileAutoComplete;
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
        }*/
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
                new Language(){Key =  "M", Value= Languages.Male},
                new Language(){Key =  "F", Value= Languages.Female}
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
