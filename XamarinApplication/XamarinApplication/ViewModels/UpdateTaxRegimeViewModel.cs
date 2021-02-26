using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class UpdateTaxRegimeViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private TaxRegime _taxRegime;
        #endregion

        #region Constructors
        public UpdateTaxRegimeViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public TaxRegime TaxRegime
        {
            get { return _taxRegime; }
            set
            {
                _taxRegime = value;
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
        public async void EditTaxRegime()
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
            if (string.IsNullOrEmpty(TaxRegime.code) || string.IsNullOrEmpty(TaxRegime.description))
            {
                Value = true;
                return;
            }
            var template = new TaxRegime
            {
                id = TaxRegime.id,
                code = TaxRegime.code,
                description = TaxRegime.description
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<TaxRegime>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/taxRegime/update",
            res,
            template);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            TaxRegimeViewModel.GetInstance().Update(template);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Tax Regime Updated");
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
                    EditTaxRegime();
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
    }
}
