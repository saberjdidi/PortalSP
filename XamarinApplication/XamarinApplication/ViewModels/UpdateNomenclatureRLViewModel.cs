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
    public class UpdateNomenclatureRLViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private NomenclatureRL _nomenclatureRL;
        #endregion

        #region Constructors
        public UpdateNomenclatureRLViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public NomenclatureRL NomenclatureRL
        {
            get { return _nomenclatureRL; }
            set
            {
                _nomenclatureRL = value;
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
        public async void EditNomenclatureRL()
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
            if (string.IsNullOrEmpty(NomenclatureRL.code) || string.IsNullOrEmpty(NomenclatureRL.description))
            {
                Value = true;
                return;
            }
            var nomenclature = new NomenclatureRL
            {
                id = NomenclatureRL.id,
                code = NomenclatureRL.code,
                description = NomenclatureRL.description,
                note = NomenclatureRL.note
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<NomenclatureRL>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/nomenclatureRL/update",
            res,
            nomenclature);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            NomenclatureRLViewModel.GetInstance().Update(nomenclature);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "NomenclatureRL Updated");
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
                    EditNomenclatureRL();
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
