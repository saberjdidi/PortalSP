using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
    public class NewServiceViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewServiceViewModel()
        {
            apiService = new ApiServices();
            ListUserAutoComplete();
            SelectedUser = new List<UserWrapper>();
        }
        #endregion

        #region Properties
        public string Code { get; set; }
        public string Description { get; set; }
        public string ZipCode { get; set; }
        public string Domicile { get; set; }
        public string Residence { get; set; }
        public string Phone { get; set; }
        public string TVACode { get; set; }
        private DateTime _birthDate = System.DateTime.Today;  //DateTime.Today.Date;
        public DateTime BirthDate
        {
            get { return _birthDate; }
            set
            {
                _birthDate = value;
                OnPropertyChanged();
            }
        }
        //public User User { get; set; }
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
        public async void AddNewService()
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
            if (string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(ZipCode) || string.IsNullOrEmpty(Domicile))
            {
                Value = true;
                return;
            }
            var _ambulatory = new AddAmbulatory
            {
                code = Code,
                description = Description,
                zipCode = ZipCode,
                domicile = Domicile,
                residence = Residence,
                phone = Phone,
                tvaCode = TVACode,
                birthDate = System.DateTime.Today
            };
            var _service = new Service
            {
                ambulatory = _ambulatory,
                usresWrapper = SelectedUser
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<Service>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/ambulatory/save",
            res,
            _service);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Service Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand SaveService
        {
            get
            {
                return new Command(() =>
                {
                    AddNewService();
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
