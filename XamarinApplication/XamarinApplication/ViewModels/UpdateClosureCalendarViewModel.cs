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
   public class UpdateClosureCalendarViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private ClosureCalendar _closureCalendar;
        #endregion

        #region Constructors
        public UpdateClosureCalendarViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public ClosureCalendar ClosureCalendar
        {
            get { return _closureCalendar; }
            set
            {
                _closureCalendar = value;
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
        public async void EditClosureCalendar()
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
            if (string.IsNullOrEmpty(ClosureCalendar.code) || string.IsNullOrEmpty(ClosureCalendar.period))
            {
                Value = true;
                return;
            }
            var closureCalendar = new ClosureCalendar
            {
                id = ClosureCalendar.id,
                code = ClosureCalendar.code,
                period = ClosureCalendar.period,
                startDate = ClosureCalendar.startDate,
                endDate = ClosureCalendar.endDate
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<ClosureCalendar>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/closurePeriod/update",
            res,
            closureCalendar);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            ClosureCalendarViewModel.GetInstance().Update(closureCalendar);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Closure Calendar Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand UpdateClosureCalendar
        {
            get
            {
                return new Command(() =>
                {
                    EditClosureCalendar();
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
