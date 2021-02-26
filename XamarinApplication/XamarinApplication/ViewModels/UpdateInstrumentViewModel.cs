using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class UpdateInstrumentViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Instrument _instrument;
        #endregion

        #region Constructors
        public UpdateInstrumentViewModel()
        {
            apiService = new ApiServices();
            ListType = GetType().OrderBy(t => t.Value).ToList();
        }
        #endregion

        #region Properties
        public Instrument Instrument
        {
            get { return _instrument; }
            set
            {
                _instrument = value;
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
        public async void EditInstrument()
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
            if (string.IsNullOrEmpty(Instrument.code) || string.IsNullOrEmpty(Instrument.name))
            {
                Value = true;
                return;
            }
            if (SelectedType == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please Select Type", "ok");
                Value = true;
                return;
            }
            var instrument = new InstrumentUpdate
            {
                id = Instrument.id,
                active = Instrument.active,
                code = Instrument.code,
                name = Instrument.name,
                description = Instrument.description,
                type = SelectedType.Key,
                createBy = Instrument.createBy,
                deleteBy = Instrument.deleteBy,
                //createDate = Room.createDate.ToString(),
                //deleteDate = Room.deleteDate.ToString(),
                deleteReason = Instrument.deleteReason
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<InstrumentUpdate>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/instrument/update",
            res,
            instrument);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            /* if (!response.IsSuccess)
             {
                 await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                 return;
             }*/
            Value = false;
            InstrumentViewModel.GetInstance().Update(Instrument);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Instrument Updated");
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
                    EditInstrument();
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
