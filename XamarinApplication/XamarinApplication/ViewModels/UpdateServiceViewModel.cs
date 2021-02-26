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
    public class UpdateServiceViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private ServiceUpdate _service;
        #endregion

        #region Constructors
        public UpdateServiceViewModel()
        {
            GetService();
            apiService = new ApiServices();
            ListUserAutoComplete();
            SelectedUser = new List<UserWrapper>();
        }
        #endregion

        #region Properties
        public int IdService { get; set; }
        public Ambulatory Ambulatory { get; set; }
        public ServiceUpdate Service
        {
            get { return _service; }
            set
            {
                _service = value;
                OnPropertyChanged();
            }
        }
        private object selectedUser;
        public object SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                // RaisePropertyChanged("SelectedItem");
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
        public async void GetService()
        {

            MessagingCenter.Subscribe<PassIdPatient>(this, "UpdateServiceId", async (value) =>
            {
                IdService = value.idPatient;
                Debug.WriteLine("********Id of service*************");
                Debug.WriteLine(IdService);

                var timestamp = DateTime.Now.ToFileTime();
                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/ambulatory/getById?id=" + IdService + "&time=" + timestamp;
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
                var list = JsonConvert.DeserializeObject<ServiceUpdate>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********list*************");
                Debug.WriteLine(list);
                Service = (ServiceUpdate)list;
            });
        }
        public async void EditService()
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
            if (string.IsNullOrEmpty(Ambulatory.code) || string.IsNullOrEmpty(Ambulatory.description))
            {
                Value = true;
                return;
            }
            var _ambulatory = new Ambulatory
            {
                id = Service.ambulatory.id,
                code = Service.ambulatory.code,
                description = Service.ambulatory.description,
                zipCode = Service.ambulatory.zipCode,
                domicile = Service.ambulatory.domicile,
                residence = Service.ambulatory.residence,
                phone = Service.ambulatory.phone,
                tvaCode = Service.ambulatory.tvaCode
            };
            var _service = new ServiceUpdate
            {
                ambulatory = _ambulatory,
                usresWrapper = Service.usresWrapper
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<ServiceUpdate>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/ambulatory/update",
            res,
            _service);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            ServiceViewModel.GetInstance().Update(_ambulatory);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Service Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand UpdateService
        {
            get
            {
                return new Command(() =>
                {
                    EditService();
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
        //User
        private List<UserWrapper> _userAutoComplete;
        public List<UserWrapper> UserAutoComplete
        {
            get { return _userAutoComplete; }
            set
            {
                _userAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<UserWrapper>> ListUserAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                criteria1 = "",
                order = "asc",
                sortedBy = "userName"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<UserWrapper>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/ambulatory/searchUsers",
            res,
            _searchModel);
            UserAutoComplete = (List<UserWrapper>)response.Result;
            return UserAutoComplete;
        }
        #endregion
    }
}
