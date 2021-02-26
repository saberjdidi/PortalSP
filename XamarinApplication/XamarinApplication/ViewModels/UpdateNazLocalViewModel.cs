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
   public class UpdateNazLocalViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private NazLocal _nazLocal;
        #endregion

        #region Constructors
        public UpdateNazLocalViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public NazLocal NazLocal
        {
            get { return _nazLocal; }
            set
            {
                _nazLocal = value;
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
        public async void EditNazLocal()
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
            if (string.IsNullOrEmpty(NazLocal.code) || string.IsNullOrEmpty(NazLocal.description))
            {
                Value = true;
                return;
            }
            var nazLocal = new NazLocal
            {
                id = NazLocal.id,
                code = NazLocal.code,
                description = NazLocal.description,
                codVal = NazLocal.codVal,
                cee = NazLocal.cee,
                lunVal1 = NazLocal.lunVal1,
                lunVal2 = NazLocal.lunVal2
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<NazLocal>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/nazLocal/update",
            res,
            nazLocal);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            NazLocalViewModel.GetInstance().Update(nazLocal);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Naz Local Updated");
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
                    EditNazLocal();
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
