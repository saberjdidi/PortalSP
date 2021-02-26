using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class InstrumentHistoricViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<InstrumentHistoric> _instrument;
        private List<InstrumentHistoric> instrumentList;
        bool _isVisibleStatus;
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructors
        public InstrumentHistoricViewModel()
        {
            apiService = new ApiServices();
            GetInstrumentHistoric();
        }
        #endregion

        #region Properties
        public Instrument Instrument { get; set; }
        public ObservableCollection<InstrumentHistoric> Instruments
        {
            get { return _instrument; }
            set
            {
                _instrument = value;
                OnPropertyChanged();
            }
        }
        public bool IsVisibleStatus
        {
            get { return _isVisibleStatus; }
            set
            {
                _isVisibleStatus = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void GetInstrumentHistoric()
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
            var instrument = new Instrument
            {
                id = Instrument.id,
                code = Instrument.code,
                description = Instrument.description,
                active = Instrument.active,
                name = Instrument.name,
                createBy = Instrument.createBy,
                createDate = Instrument.createDate,
                deleteBy = Instrument.deleteBy,
                deleteDate = Instrument.deleteDate,
                deleteReason = Instrument.deleteReason
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.InstrumentHistoric<InstrumentHistoric>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/instrumentHistoric/getInstrumentHistoric",
            res,
            instrument);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            instrumentList = (List<InstrumentHistoric>)response.Result;
            Instruments = new ObservableCollection<InstrumentHistoric>(instrumentList);
            if (Instruments.Count() == 0)
            {
                IsVisibleStatus = true;
            }
            else
            {
                IsVisibleStatus = false;
            }
        }
        #endregion

        #region Commands
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
