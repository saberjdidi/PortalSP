using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class ConventionDeactivateViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Properties
        public INavigation Navigation { get; set; }
        public Convention Convention { get; set; }
        #endregion

        #region Constructors
        public ConventionDeactivateViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Methods
        public async void DactivateConvention()
        {
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            var _convention = new Convention
            {
                id = Convention.id,
                socialReason = Convention.socialReason,
                status = Convention.status,
                startValidation = Convention.startValidation,
                endValidation = Convention.endValidation,
                deactivationDate = Convention.deactivationDate,
                tva = Convention.tva,
                discount = Convention.discount,
                collaboratorNumber = Convention.collaboratorNumber
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.Save<Convention>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/convention/deactivate",
            res,
            _convention);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            ConventionViewModel.GetInstance().Update(_convention);
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Convention Disactivated");
            await PopupNavigation.Instance.PopAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Deactivate
        {
            get
            {
                return new Command(() =>
                {
                    DactivateConvention();
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
    }
}
