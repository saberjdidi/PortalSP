using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
    public class NewDoctorViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewDoctorViewModel()
        {
            apiService = new ApiServices();

            ListResidenceAutoComplete();
            ListClientAutoComplete();
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
        public string Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FiscalCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        private string _birthDate = DateTime.Now.ToString("MM/dd/yyyy");  //System.DateTime.Today
        public string BirthDate
        {
            get { return _birthDate; }
            set
            {
                _birthDate = value;
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
        #endregion

        #region Methods
        public async void AddNewDoctor()
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
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(FiscalCode) || string.IsNullOrEmpty(Phone))
            {
                Value = true;
                return;
            }
            if (Client == null || Residence == null)
            {
                Value = true;
                return;
            }
            var _residence = new Residence
            {
                comuniLocal = Residence
            };
            var doctor = new AddDoctor
            {
                code = Code,
                firstName = FirstName,
                lastName = LastName,
                fiscalCode = FiscalCode,
                phone = Phone,
                birthDate = BirthDate,
                email = Email,
                residence = _residence,
                client = Client
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddDoctor>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/doctor/save",
            res,
            doctor);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            /*if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }*/
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Doctor Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand SaveDoctor
        {
            get
            {
                return new Command(() =>
                {
                    AddNewDoctor();
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
        //Residence
        private List<ComuniLocal> _comuniLocal;
        public List<ComuniLocal> ResidenceAutoComplete
        {
            get { return _comuniLocal; }
            set
            {
                _comuniLocal = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<ComuniLocal>> ListResidenceAutoComplete()
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
            ResidenceAutoComplete = (List<ComuniLocal>)response.Result;
            return ResidenceAutoComplete;
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
    }
}
