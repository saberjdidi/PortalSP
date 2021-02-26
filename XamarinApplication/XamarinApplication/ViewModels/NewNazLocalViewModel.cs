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
   public class NewNazLocalViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewNazLocalViewModel()
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
        public string Code { get; set; }
        public string Description { get; set; }
        public string CodVal { get; set; }
        public int LunVal1 { get; set; }
        public int LunVal2 { get; set; }
        private bool _cee = false;
        public bool CEE
        {
            get { return _cee; }
            set
            {
                this._cee = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void AddNazLocal()
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
            var nazLocal = new AddNazLocal
            {
                code = Code,
                description = Description,
                codVal = CodVal,
                lunVal1 = LunVal1,
                lunVal2 = LunVal2,
                cee = CEE
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddNazLocal>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/nazLocal/save",
            res,
            nazLocal);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Naz Local Added");
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
                    AddNazLocal();
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
