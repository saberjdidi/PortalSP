using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class UpdateJobCronViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private Configs _configs;
        #endregion

        #region Constructors
        public UpdateJobCronViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public Configs Configs
        {
            get { return _configs; }
            set
            {
                _configs = value;
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
        public async void EditJobCron()
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
            if (string.IsNullOrEmpty(Configs.cron))
            {
                Value = true;
                return;
            }
            var config = new Configs
            {
                id = Configs.id,
                code = Configs.code,
                cron = Configs.cron
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<Configs>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/mailSender/updateCron",
            res,
            config);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            // ICDOViewModel.GetInstance().Update(icdo);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Job Cron Updated");
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
                   // EditJobCron();
                    Debug.WriteLine("********cron*************");
                    Debug.WriteLine(Configs.cron);
                    string str = Configs.cron;
                    int index = str.IndexOf(' ');
                    Debug.WriteLine("********index*************");
                    Debug.WriteLine(index);
                    index = str.IndexOf(' ', index + 1);
                    Debug.WriteLine("********index2*************");
                    Debug.WriteLine(index);
                    /* for (int i = 0; i < str.Length; i++)
                     {
                         int minute = str[2];
                         Debug.WriteLine("********minute*************");
                         Debug.WriteLine(minute);
                         int hour = str[5]+ str[6];
                         Debug.WriteLine("********hour*************");
                         Debug.WriteLine(hour);
                     }*/
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
