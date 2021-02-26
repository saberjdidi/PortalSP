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

namespace XamarinApplication.ViewModels
{
   public class AmbulatoryRequestRoleViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        public Attachment Attachment { get; set; }
        private AmbulatoryRequest _ambulatoryRequest;
        private bool value = true;
        #endregion

        #region Properties
        public AmbulatoryRequest AmbulatoryRequest
        {
            get { return _ambulatoryRequest; }
            set
            {
                _ambulatoryRequest = value;
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
        private Ambulatory _ambulatory = null;
        public Ambulatory Ambulatory
        {
            get { return _ambulatory; }
            set
            {
                _ambulatory = value;
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

        #region Constructors
        public AmbulatoryRequestRoleViewModel()
        {
            apiService = new ApiServices();
            GetAmbulatoryRequest();
            ListAmbulatoryAutoComplete();
        }
        #endregion

        #region Methods
        public async void GetAmbulatoryRequest()
        {
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
            var response = await apiService.GetAttachmentWithCoockie<AmbulatoryRequest>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/ambulatoryRequest/getByRequest?requestId=" + Attachment.requests.Select(r => r.id).FirstOrDefault(),
                 res);
            /* if (!response.IsSuccess)
             {
                 await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                 return;
             }*/
            AmbulatoryRequest = (AmbulatoryRequest)response.Result;
            if (AmbulatoryRequest == null)
            {
                Ambulatory = Ambulatory;
            }
            else
            {
                Ambulatory = AmbulatoryRequest.ambulatory;
            }
        }
        public async void AddAmbulatory()
        {
            //Value = false;
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            if (Ambulatory == null)
            {
                // Value = false;
                await Application.Current.MainPage.DisplayAlert("Warning", "Select Ambulatory", "ok");
                return;
            }
            var Username = Settings.Username;
            User = JsonConvert.DeserializeObject<User>(Username);
            Debug.WriteLine("********user*************");
            Debug.WriteLine(User.id);
            Debug.WriteLine("********AmbulatoryRequest.ambulatory.id*************");
            Debug.WriteLine(Ambulatory.id);
            Debug.WriteLine("********Request.id*************");
            Debug.WriteLine(Attachment.requests.Select(r => r.id).FirstOrDefault());

            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            // var url = "https://portalesp.smart-path.it/Portalesp/ambulatoryRequest/create?ampId="+ Ambulatory.id + "&reqId="+ Request.id + "&usId="+ User.id + "&dt=Tue%20Jan%2005%202021%2001:00:00%20GMT+0100%20(heure%20normale%20d%E2%80%99Europe%20centrale)";
            var url = "https://portalesp.smart-path.it/Portalesp/ambulatoryRequest/create?ampId=" + Ambulatory.id + "&reqId=" + Attachment.requests.Select(r => r.id).FirstOrDefault() + "&usId=" + User.id + "&dt=" + DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
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
            //Value = true;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Ambulatory Saved");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        public async void DeleteAmbulatory()
        {

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }
            if (AmbulatoryRequest == null)
            {
                // Value = false;
                await Application.Current.MainPage.DisplayAlert("Warning", "Ambulatory Request is Empty", "ok");
                return;
            }

            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.Delete<AmbulatoryRequest>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/ambulatoryRequest/delete/" + AmbulatoryRequest.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Ambulatory Deleted");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Save
        {
            get
            {
                return new Command(() =>
                {
                    AddAmbulatory();
                });
            }
        }
        public ICommand Delete
        {
            get
            {
                return new Command(() =>
                {
                    /*  var response = await dialogService.ShowConfirm(
                  "Confirm",
                  Languages.ConfirmationDelete + " Ambulatory Request ?");
                      if (!response)
                      {
                          return;
                      }*/
                    DeleteAmbulatory();
                });
            }
        }
        #endregion

        #region Autocomplete
        //Service
        private List<Ambulatory> _ambulatoryAutoComplete;
        public List<Ambulatory> AmbulatoryAutoComplete
        {
            get { return _ambulatoryAutoComplete; }
            set
            {
                _ambulatoryAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Ambulatory>> ListAmbulatoryAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                order = "asc",
                sortedBy = "code"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Ambulatory>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/ambulatory/search",
            res,
            _searchModel);
            AmbulatoryAutoComplete = (List<Ambulatory>)response.Result;
            return AmbulatoryAutoComplete;
        }
        #endregion
    }
}
