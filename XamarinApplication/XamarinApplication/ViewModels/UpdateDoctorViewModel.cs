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
    public class UpdateDoctorViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Doctor _doctor;
        #endregion

        #region Constructors
        public UpdateDoctorViewModel()
        {
            apiService = new ApiServices();
            GetDoctor();

            ListResidenceAutoComplete();
            ListClientAutoComplete();
        }
        #endregion

        #region Properties
        public int IdDoctor { get; set; }
        public Doctor DoctorId { get; set; }
        public Doctor Doctor
        {
            get { return _doctor; }
            set
            {
                _doctor = value;
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
        public async void GetDoctor()
        {

            MessagingCenter.Subscribe<PassIdPatient>(this, "UpdateDoctorId", async (value) =>
            {
                IdDoctor = value.idPatient;
                Debug.WriteLine("********IdDoctor*************");
                Debug.WriteLine(IdDoctor);

                var timestamp = DateTime.Now.ToFileTime();
                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/doctor/getById?id=" + IdDoctor + "&time=" + timestamp;
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
                var list = JsonConvert.DeserializeObject<Doctor>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********list*************");
                Debug.WriteLine(list);
                Doctor = (Doctor)list;
            });
        }
        public async void EditDoctor()
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
            if (string.IsNullOrEmpty(Doctor.code) || string.IsNullOrEmpty(Doctor.firstName) || string.IsNullOrEmpty(Doctor.lastName) ||
                string.IsNullOrEmpty(Doctor.fiscalCode))
            {
                Value = true;
                return;
            }
            var _residence = new Residence
            {
                id = Doctor.residence.id,
                comuniLocal = Doctor.residence.comuniLocal,
                street = Doctor.residence.street
            };
            var doctor = new Doctor
            {
                id = Doctor.id,
                code = Doctor.code,
                firstName = Doctor.firstName,
                lastName = Doctor.lastName,
                fiscalCode = Doctor.fiscalCode,
                phone = Doctor.phone,
                email = Doctor.email,
                client = Doctor.client,
                residence = _residence,
                birthDate = Doctor.birthDate
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<Doctor>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/doctor/update",
            res,
            doctor);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            DoctorViewModel.GetInstance().Update(doctor);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Doctor Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand UpdateDoctor
        {
            get
            {
                return new Command(() =>
                {
                    EditDoctor();
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
                criteria1 = "",
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
