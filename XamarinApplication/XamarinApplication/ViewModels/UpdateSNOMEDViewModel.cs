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
    public class UpdateSNOMEDViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Snomed _snomed;
        #endregion

        #region Constructors
        public UpdateSNOMEDViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public Snomed Snomed
        {
            get { return _snomed; }
            set
            {
                _snomed = value;
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
        public async void EditSNOMED()
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
            if (string.IsNullOrEmpty(Snomed.code) || string.IsNullOrEmpty(Snomed.description))
            {
                Value = true;
                return;
            }
            var snomed = new Snomed
            {
                id = Snomed.id,
                code = Snomed.code,
                description = Snomed.description
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<Snomed>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/snomed/update",
            res,
            snomed);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            SNOMEDViewModel.GetInstance().Update(snomed);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "SNOMED Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand UpdateSNOMED
        {
            get
            {
                return new Command(() =>
                {
                    EditSNOMED();
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
