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
   public class UpdateConventionViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Convention _convention;
        #endregion

        #region Constructors
        public UpdateConventionViewModel()
        {
            apiService = new ApiServices();
            ListTVAAutoComplete();
        }
        #endregion

        #region Properties
        public Convention Convention
        {
            get { return _convention; }
            set
            {
                _convention = value;
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
        public async void EditConvention()
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
            if (string.IsNullOrEmpty(Convention.socialReason))
            {
                Value = true;
                return;
            }
           /* if (Convention.tva == null)
            {
                Value = true;
                await Application.Current.MainPage.DisplayAlert("Alert", "TVA is required", "ok");
                return;
            }*/
            var convention = new UpdateConvention
            {
                id = Convention.id,
                socialReason = Convention.socialReason,
                startValidation = Convention.startValidation.ToString("dd/MM/yyyy"),
                endValidation = Convention.endValidation.ToString("dd/MM/yyyy"),
                deactivationDate = Convention.deactivationDate.ToString("dd/MM/yyyy"),
                tva = Convention.tva,
                discount = Convention.discount,
                collaboratorNumber = Convention.collaboratorNumber,
                status = Convention.status.name
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<UpdateConvention>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/convention/update",
            res,
            convention);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
           /* if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }*/
            Value = false;
            // ConventionViewModel.GetInstance().Update(convention);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Convention Updated");
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
                    EditConvention();
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
