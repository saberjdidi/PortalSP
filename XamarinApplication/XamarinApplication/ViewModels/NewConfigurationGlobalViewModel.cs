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
   public class NewConfigurationGlobalViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewConfigurationGlobalViewModel()
        {
            apiService = new ApiServices();

            ListNomenclaturaAutoComplete();
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
        public string Description { get; set; }
        private Nomenclatura _nomenclatura = null;
        public Nomenclatura Nomenclatura
        {
            get { return _nomenclatura; }
            set
            {
                _nomenclatura = value;
                OnPropertyChanged();
            }
        }
        private bool _showHide = false;
        public bool ShowHide
        {
            get => _showHide;
            set
            {
                _showHide = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void AddGlobalConfig()
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
            if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(Code))
            {
                Value = true;
                return;
            }
            if (Nomenclatura == null)
            {
                Value = true;
                await Application.Current.MainPage.DisplayAlert("Warning", "Nomenclatura is required", "ok");
                return;
            }
            var conventionConfig = new ConventionGlobalConfig
            {
                code = Code,
                description = Description
            };
            var _nomenclatureBilling = new List<NomenclatureBilling>()
                       {
                           new NomenclatureBilling
                                {
                                    nomenclatura = Nomenclatura,
                                    price = Nomenclatura.ptsyn
                                }
                       };
            var addConfig = new AddConventionGlobalConfig
            {
                conventionGlobalConfig = conventionConfig,
                nomenclatureBillings = _nomenclatureBilling
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddConventionGlobalConfig>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/conventionGlobalConfig/save",
            res,
            addConfig);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Global Configuration Added");
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
                    AddGlobalConfig();
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
        public ICommand OpenNomenclaturaCommand
        {
            get
            {
                return new Command(() =>
                {
                    if (Nomenclatura == null)
                    {
                        Application.Current.MainPage.DisplayAlert("Alert", "Please Select Nomenclatura", "ok");
                        return;
                    }
                    else
                    {
                        ShowHide = true;
                    }
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
