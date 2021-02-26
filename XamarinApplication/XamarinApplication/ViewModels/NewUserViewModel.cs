using Rg.Plugins.Popup.Extensions;
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
    public class NewUserViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewUserViewModel()
        {
            apiService = new ApiServices();

            ListClientAutoComplete();
            ListRoleAutoComplete();
            ListType = GetTypes().ToList();
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
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FiscalCode { get; set; }
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
        private Role _role = null;
        public Role Role
        {
            get { return _role; }
            set
            {
                _role = value;
                OnPropertyChanged();
            }
        }
        private bool _enable = true;
        public bool Enable
        {
            get { return _enable; }
            set
            {
                this._enable = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void AddNewUser()
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
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
            {
                Value = true;
                return;
            }
            if (Client == null || Role == null)
            {
                Value = true;
                return;
            }
           /* var _typeRole = new TypeRole
            {
                name = "OPERATOR",
                enumType = "eu.smartpath.portalesp.enums.Type"
            };
            var _role = new Role
            {
                id = 2,
                authority = "OPERATOR_ROLE",
                type = _typeRole
            };*/
            var user = new AddUser
            {
                firstName = FirstName,
                lastName = LastName,
                userName = UserName,
                password = Password,
                email = Email,
                fiscalCode = FiscalCode,
                enabled = Enable,
                client = Client,
                role = Role
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddUser>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/user/save",
            res,
            user);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            /*if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }*/
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "User Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand SaveUser
        {
            get
            {
                return new Command(() =>
                {
                    AddNewUser();
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
