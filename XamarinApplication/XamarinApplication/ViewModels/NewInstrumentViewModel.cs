using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class NewInstrumentViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewInstrumentViewModel()
        {
            apiService = new ApiServices();
            ListType = GetType().OrderBy(t => t.Value).ToList();
        }
        #endregion

        #region Properties
        public string Name { get; set; }
        public string Description { get; set; }

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
        private bool _active = true;
        public bool Active
        {
            get { return _active; }
            set
            {
                this._active = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void AddNewInstrument()
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
            if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Description))
            {
                Value = true;
                return;
            }
            if (SelectedType.Key == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please Select Type", "ok");
                return;
            }
            var _instrument = new AddInstrument
            {
                active = Active,
                name = Name,
                description = Description,
                type = SelectedType.Key
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddInstrument>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/instrument/save",
            res,
            _instrument);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Instrument Added");
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
                    AddNewInstrument();
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

        #region type
        public List<Language> ListType { get; set; }
        private Language _selectedType { get; set; }
        public Language SelectedType
        {
            get { return _selectedType; }
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<Language> GetType()
        {
            var languages = new List<Language>()
            {
                new Language(){Key =  "MA", Value= "Mammography"},
                new Language(){Key =  "CO", Value= "Colposcope"},
                new Language(){Key =  "SC", Value= "Scan"},
                new Language(){Key =  "OT", Value= "Other"}
            };

            return languages;
        }
        #endregion
    }
}
