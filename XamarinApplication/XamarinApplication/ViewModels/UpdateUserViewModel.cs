using Newtonsoft.Json;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    public class UpdateUserViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private User _user;
        private bool isRefreshing;
        #endregion

        #region Constructors
        public UpdateUserViewModel()
        {
            apiService = new ApiServices();
            GetUser();

            ListClientAutoComplete();
            ListRoleAutoComplete();
            ListTitleAutoComplete();
            ListType = GetTypes().ToList();
        }
        #endregion

        #region Properties
        public int IdUser { get; set; }
        public User UserId { get; set; }
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
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
        public bool IsRefreshing
        {
            get
            {
                return isRefreshing;
            }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void GetUser()
        {

            MessagingCenter.Subscribe<PassIdPatient>(this, "UpdateUserId", async (value) =>
            {
                IdUser = value.idPatient;
                Debug.WriteLine("********Id of user*************");
                Debug.WriteLine(IdUser);

                var timestamp = DateTime.Now.ToFileTime();
                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/user/getById?id=" + IdUser + "&time=" + timestamp;
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
                var list = JsonConvert.DeserializeObject<User>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********list*************");
                Debug.WriteLine(list);
                User = (User)list;
            });
        }
        public async void EditUser()
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
             if (string.IsNullOrEmpty(User.userName) || string.IsNullOrEmpty(User.firstName) || string.IsNullOrEmpty(User.lastName) ||
                 string.IsNullOrEmpty(User.password))
             {
                 Value = true;
                 return;
             }
             if (User.client == null || User.role == null)
             {
                 Value = true;
                 return;
             }
             var _user = new User
             {
                 id = User.id,
                 firstName = User.firstName,
                 lastName = User.lastName,
                 email = User.email,
                 userName = User.userName,
                 password = User.password,
                 fiscalCode = User.fiscalCode,
                 client = User.client,
                 creationDate = User.creationDate,  //DateTime.Now.ToString("yyyy’-‘MM’-‘dd’T’HH’:’mm’:’ss.fffffffK"),
                 enabled = User.enabled,
                 role = User.role
             };
             var cookie = Settings.Cookie;  //.Split(11, 33)
             var res = cookie.Substring(11, 32);

             var response = await apiService.PutUser<User>(
             "https://portalesp.smart-path.it",
             "/Portalesp",
             "/user/update",
             res,
             _user);
             Debug.WriteLine("********responseIn ViewModel*************");
             Debug.WriteLine(response);
              if (!response.IsSuccess)
              {
                  await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                  return;
              }
              Value = false;
              UserViewModel.GetInstance().Update(_user);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "User Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand UpdateUser
        {
            get
            {
                return new Command(() =>
                {
                    EditUser();
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
        public ICommand DefaultTemplateReport
        {
            get
            {
                return new Command(async () =>
                {
                    var cookie = Settings.Cookie;
                    var res = cookie.Substring(11, 32);
                    var cookieContainer = new CookieContainer();
                    var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                    var client = new HttpClient(handler);
                    var url = "https://portalesp.smart-path.it/Portalesp/doctorTemplate/downloadDoctorTemplate?id="+ User.id;
                    Debug.WriteLine("********url*************");
                    Debug.WriteLine(url);
                    PopuPage page1 = new PopuPage();
                    await PopupNavigation.Instance.PushAsync(page1);
                    //await Task.Delay(5000);
                    
                    client.BaseAddress = new Uri(url);
                    cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                        return;
                    }
                    var result = await response.Content.ReadAsStreamAsync();
                    Debug.WriteLine("********result*************");
                    Debug.WriteLine(result);
                    await App.Current.MainPage.Navigation.PopPopupAsync(true);
                    using (var streamReader = new MemoryStream())
                    {
                        result.CopyTo(streamReader);
                        byte[] bytes = streamReader.ToArray();
                        MemoryStream stream = new MemoryStream(bytes);
                        Debug.WriteLine("********stream*************");
                        Debug.WriteLine(stream);
                        if (stream == null)
                        {
                            await Application.Current.MainPage.DisplayAlert("Warning", "Data is Empty", "ok");
                            return;
                        }

                        await DependencyService.Get<ISave>().SaveAndView("defaultTemplate" + ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", stream);
                    }
                    
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
        //Role
        private List<Role> _roleAutoComplete;
        public List<Role> RoleAutoComplete
        {
            get { return _roleAutoComplete; }
            set
            {
                _roleAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Role>> ListRoleAutoComplete()
        {
            var timestamp = DateTime.Now.ToFileTime();
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<Role>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/role/list?sortedBy=authority&order=asc&time=" + timestamp,
                 res);
            RoleAutoComplete = (List<Role>)response.Result;
            return RoleAutoComplete;
        }
        //title
        public List<string> StringAutoComplete { get; set; }
        public void ListTitleAutoComplete()
        {
            var titles = new List<string>() { "Operator", "Administration", "Doctor", "Technical", "Service", "Visitor" };
            StringAutoComplete = new List<string>(titles);
        }
        #endregion

        #region Status
        //*********Title**************
        public List<Language> ListType { get; set; }
        private Language _selectedType { get; set; }
        public Language SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<Language> GetTypes()
        {
            var languages = new List<Language>()
            {
                new Language(){Key =  "Operator", Value= "Operator"},
                new Language(){Key =  "Administration", Value= "Administration"},
                new Language(){Key =  "Doctor", Value= "Doctor"},
                new Language(){Key =  "Technical", Value= "Technical"},
                new Language(){Key =  "Service", Value= "Service"},
                new Language(){Key =  "Visitor", Value= "Visitor"}
            };

            return languages;
        }
        #endregion
    }
}
