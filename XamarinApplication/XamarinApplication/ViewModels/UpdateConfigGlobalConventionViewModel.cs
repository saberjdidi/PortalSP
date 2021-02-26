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
   public class UpdateConfigGlobalConventionViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private ConfigurationConvention _configurationConvention;
        private UpdateConventionGlobalConfig _updateConvention;
        #endregion

        #region Constructors
        public UpdateConfigGlobalConventionViewModel()
        {
            apiService = new ApiServices();
            ListNomenclaturaAutoComplete();
            GetConventionById();
        }
        #endregion

        #region Properties
        public ConfigurationConvention ConfigurationConvention
        {
            get { return _configurationConvention; }
            set
            {
                _configurationConvention = value;
                OnPropertyChanged();
            }
        }
        public UpdateConventionGlobalConfig UpdateConvention
        {
            get { return _updateConvention; }
            set
            {
                _updateConvention = value;
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
        public async void GetConventionById()
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
            var timestamp = DateTime.Now.ToFileTime();
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetAttachmentWithCoockie<UpdateConventionGlobalConfig>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/conventionGlobalConfig/getById?id=" + ConfigurationConvention.id + "&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            UpdateConvention = (UpdateConventionGlobalConfig)response.Result;
        }
        public async void EditConfig()
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
            if (string.IsNullOrEmpty(UpdateConvention.conventionGlobalConfig.code) || string.IsNullOrEmpty(UpdateConvention.conventionGlobalConfig.description))
            {
                Value = true;
                return;
            }
            var _configConvention = new ConfigurationConvention
            {
                id = UpdateConvention.conventionGlobalConfig.id,
                code = UpdateConvention.conventionGlobalConfig.code,
                description = UpdateConvention.conventionGlobalConfig.description
            };
            var _nomenclatureBilling = new List<NomenclatureBilling>
                       {
                           new NomenclatureBilling
                                {
                                    nomenclatura = UpdateConvention.nomenclatureBillings.Select(n => n.nomenclatura).FirstOrDefault(),  //.ForEach( n=>{n.nomenclatura } ),
                                    price = UpdateConvention.nomenclatureBillings.Select(p => p.price).FirstOrDefault()  //.ForEach(p => { p.price })
                                }
                       };
            
            var updateConfig = new UpdateConventionGlobalConfig
            {
                conventionGlobalConfig = _configConvention,
                nomenclatureBillings = _nomenclatureBilling //UpdateConvention.nomenclatureBillings
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<UpdateConventionGlobalConfig>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/conventionGlobalConfig/update",
            res,
            updateConfig);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            /*if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }*/
            Value = false;
            // ConfigurationGlobaleConventionViewModel.GetInstance().Update(_configConvention);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Convention Global Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Update
        {
            get
            {
                return new Command(() =>
                {
                    EditConfig();
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
        //Procedure(Nomenclature)
        private List<Nomenclatura> _nomenclatureAutoComplete;
        public List<Nomenclatura> NomenclaturAutoComplete
        {
            get { return _nomenclatureAutoComplete; }
            set
            {
                _nomenclatureAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Nomenclatura>> ListNomenclaturaAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                criteria1 = "",
                order = "asc",
                sortedBy = "code",
                check1 = true,
                fromReflex = false
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Nomenclatura>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/nomenclatura/advancedSearch",
            res,
            _searchModel);
            NomenclaturAutoComplete = (List<Nomenclatura>)response.Result;
            return NomenclaturAutoComplete;
        }
        #endregion
    }
}
