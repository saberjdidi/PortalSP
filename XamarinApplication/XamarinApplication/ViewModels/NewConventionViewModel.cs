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
   public class NewConventionViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewConventionViewModel()
        {
            apiService = new ApiServices();

            ListTVAAutoComplete();
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
        public string SocialReason { get; set; }
        public int Discount { get; set; }
        public TVA TVA { get; set; }
        private string _startValidation = new DateTime(2016, 01, 02).ToString("MM/dd/yyyy");
        public string StartValidation
        {
            get { return _startValidation; }
            set
            {
                _startValidation = value;
                OnPropertyChanged();
            }
        }
        private string _endValidation = new DateTime(2028, 12, 31).ToString("MM/dd/yyyy"); //System.DateTime.Today;  
        public string EndValidation
        {
            get { return _endValidation; }
            set
            {
                _endValidation = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void AddConvention()
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
            if (string.IsNullOrEmpty(SocialReason) || string.IsNullOrEmpty(Discount.ToString()))
            {
                Value = true;
                return;
            }
            if(TVA == null)
            {
                Value = true;
                await Application.Current.MainPage.DisplayAlert("Alert", "TVA is Required", "ok");
                return;
            }
            var convention = new AddConvention
            {
                socialReason = SocialReason,
                tva = TVA,
                status = "AC",
                startValidation = StartValidation,
                endValidation = EndValidation,
                discount = Discount,
                collaboratorNumber = 0
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            await apiService.Save<AddConvention>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/convention/save",
            res,
            convention);

           /* if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }*/
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Convention Added");
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
                    AddConvention();
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
        //TVA
        private List<TVA> _tvaAutoComplete;
        public List<TVA> TVAAutoComplete
        {
            get { return _tvaAutoComplete; }
            set
            {
                _tvaAutoComplete = value;
                OnPropertyChanged();
            } 
        }
        public async Task<List<TVA>> ListTVAAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                order = "asc",
                sortedBy = "description"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<TVA>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/tvaCode/search",
            res,
            _searchModel);
            TVAAutoComplete = (List<TVA>)response.Result;
            return TVAAutoComplete;
        }
        #endregion
    }
}
