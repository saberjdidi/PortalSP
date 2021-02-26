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
   public class NewTVAViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewTVAViewModel()
        {
            apiService = new ApiServices();

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
        private bool _hasError;
        public bool HasError
        {
            get { return _hasError; }
            set
            {
                this._hasError = value;
                OnPropertyChanged();
            }
        }
        public string Code { get; set; }
        public string Description { get; set; }
        public string Valeur { get; set; }
        private bool _bollo = false;
        public bool Bollo
        {
            get { return _bollo; }
            set
            {
                this._bollo = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void AddTVA()
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
            if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(Valeur))
            {
                HasError = true;
                return;
            }
            else
            {
                HasError = false;
            }
           /* if (string.IsNullOrEmpty(Code))
            {
                Value = true;
                return;
            }*/
            var tva = new AddTVA
            {
                code = Code,
                description = Description,
                value = Valeur,
                bolla = Bollo
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddTVA>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/tvaCode/save",
            res,
            tva);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "TVA Added");
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
                    AddTVA();
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
