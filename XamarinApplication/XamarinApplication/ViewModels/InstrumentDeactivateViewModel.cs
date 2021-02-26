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
   public class InstrumentDeactivateViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Properties
        public INavigation Navigation { get; set; }
        public Instrument Instrument { get; set; }
        public string Reason { get; set; }
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

        #region Constructors
        public InstrumentDeactivateViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Methods
        public async void deactivateInstrument()
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
            if (string.IsNullOrEmpty(Reason))
            {
                Value = true;
                return;
            }
            var instrument = new InstrumentDeactivate
            {
                id = Instrument.id,
                reason = Reason
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<InstrumentDeactivate>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/instrument/deactivate",
            res,
            instrument);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            await InstrumentViewModel.GetInstance().Activate(Instrument);
            Value = false;
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Instrument Deactivated");
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
                    deactivateInstrument();
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
