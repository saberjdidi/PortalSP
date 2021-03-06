﻿using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class NewMailPostedReportViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        public string _email;
        public int _hour = 0;
        public int _minute = 0;
        public string _days;
        #endregion

        #region Constructor
        public NewMailPostedReportViewModel()
        {
            apiService = new ApiServices();
            ListHours();
            ListMinutes();
        }
        #endregion

        #region Properties
        public string Email
        {
            get { return _email; }
            set
            {
                this._email = value;
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
        public string Days
        {
            get { return _days; }
            set
            {
                this._days = value;
                OnPropertyChanged();
            }
        }
        public int Hour
        {
            get { return _hour; }
            set
            {
                this._hour = value;
                OnPropertyChanged();
            }
        }
        public int Minute
        {
            get { return _minute; }
            set
            {
                this._minute = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void NewAddress()
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
            var emailPattern = "^[a-z0-9._-]+@[a-z0-9._-]+\\.[a-z]{2,6}$";
            if (string.IsNullOrEmpty(Email))
            {
                Value = true;
                return;
            }
            if (!String.IsNullOrWhiteSpace(Email) && !(Regex.IsMatch(Email, emailPattern)))
            {
                Value = true;
                return;
            }

            List<AddAddress> addAddresses = new List<AddAddress>();
            addAddresses.Add(new AddAddress()
            {
                code = "#PostedReports",
                addressMail = Email,
                duplicate = false
            });
            var _jobCron = new AddJobCron
            {
                addresses = addAddresses
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddJobCron>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/postedReports/saveMails",
            res,
            _jobCron);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Email Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        public async void NewJobCron()
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
           /* if (string.IsNullOrEmpty(Days))
            {
                Value = true;
                return;
            }*/
            List<AddConfigs> addConfigs = new List<AddConfigs>();
            addConfigs.Add(new AddConfigs()
            {
                code = "#PostedReports",
                cron = "0 " + Minute + " " + Hour + " ? * " + Days + " *"
            });
            var _jobCron = new AddJobCron
            {
                configs = addConfigs
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddJobCron>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/postedReports/saveMails",
            res,
            _jobCron);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Job Cron Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand SaveEmail
        {
            get
            {
                return new Command(() =>
                {
                    NewAddress();
                });
            }
        }
        public ICommand SaveJobCron
        {
            get
            {
                return new Command(() =>
                {
                    NewJobCron();
                    /*  Debug.WriteLine("********Days*************");
                      Debug.WriteLine(Days);
                      Debug.WriteLine("********Hour*************");
                      Debug.WriteLine(Hour);
                      Debug.WriteLine("********Minute*************");
                      Debug.WriteLine(Minute);*/
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

        #region Autocomplete
        //Hour
        public List<ResultMax> HourComboBox { get; set; }
        public void ListHours()
        {
            var _result = new List<ResultMax>()
            {
                new ResultMax(){maxResult= 0},
                new ResultMax(){maxResult= 1},
                new ResultMax(){maxResult= 2},
                new ResultMax(){maxResult= 3},
                new ResultMax(){maxResult= 4},
                new ResultMax(){maxResult= 5},
                new ResultMax(){maxResult= 6},
                new ResultMax(){maxResult= 7},
                new ResultMax(){maxResult= 8},
                new ResultMax(){maxResult= 9},
                new ResultMax(){maxResult= 10},
                new ResultMax(){maxResult= 11},
                new ResultMax(){maxResult= 12},
                new ResultMax(){maxResult= 13},
                new ResultMax(){maxResult= 14},
                new ResultMax(){maxResult= 15},
                new ResultMax(){maxResult= 16},
                new ResultMax(){maxResult= 17},
                new ResultMax(){maxResult= 18},
                new ResultMax(){maxResult= 19},
                new ResultMax(){maxResult= 20},
                new ResultMax(){maxResult= 21},
                new ResultMax(){maxResult= 22},
                new ResultMax(){maxResult= 23}
            };
            HourComboBox = new List<ResultMax>(_result);
        }
        //Minutes
        public List<ResultMax> MinuteComboBox { get; set; }
        public void ListMinutes()
        {
            var _resultMinute = new List<ResultMax>()
            {
                new ResultMax(){maxResult= 0},
                new ResultMax(){maxResult= 1},
                new ResultMax(){maxResult= 2},
                new ResultMax(){maxResult= 3},
                new ResultMax(){maxResult= 4},
                new ResultMax(){maxResult= 5},
                new ResultMax(){maxResult= 6},
                new ResultMax(){maxResult= 7},
                new ResultMax(){maxResult= 8},
                new ResultMax(){maxResult= 9},
                new ResultMax(){maxResult= 10},
                new ResultMax(){maxResult= 11},
                new ResultMax(){maxResult= 12},
                new ResultMax(){maxResult= 13},
                new ResultMax(){maxResult= 14},
                new ResultMax(){maxResult= 15},
                new ResultMax(){maxResult= 16},
                new ResultMax(){maxResult= 17},
                new ResultMax(){maxResult= 18},
                new ResultMax(){maxResult= 19},
                new ResultMax(){maxResult= 20},
                new ResultMax(){maxResult= 21},
                new ResultMax(){maxResult= 22},
                new ResultMax(){maxResult= 23},
                new ResultMax(){maxResult= 24},
                new ResultMax(){maxResult= 25},
                new ResultMax(){maxResult= 26},
                new ResultMax(){maxResult= 27},
                new ResultMax(){maxResult= 28},
                new ResultMax(){maxResult= 29},
                new ResultMax(){maxResult= 30},
                new ResultMax(){maxResult= 31},
                new ResultMax(){maxResult= 32},
                new ResultMax(){maxResult= 33},
                new ResultMax(){maxResult= 34},
                new ResultMax(){maxResult= 35},
                new ResultMax(){maxResult= 36},
                new ResultMax(){maxResult= 37},
                new ResultMax(){maxResult= 38},
                new ResultMax(){maxResult= 39},
                new ResultMax(){maxResult= 40},
                new ResultMax(){maxResult= 41},
                new ResultMax(){maxResult= 42},
                new ResultMax(){maxResult= 43},
                new ResultMax(){maxResult= 44},
                new ResultMax(){maxResult= 45},
                new ResultMax(){maxResult= 46},
                new ResultMax(){maxResult= 47},
                new ResultMax(){maxResult= 48},
                new ResultMax(){maxResult= 49},
                new ResultMax(){maxResult= 50},
                new ResultMax(){maxResult= 51},
                new ResultMax(){maxResult= 52},
                new ResultMax(){maxResult= 53},
                new ResultMax(){maxResult= 54},
                new ResultMax(){maxResult= 55},
                new ResultMax(){maxResult= 56},
                new ResultMax(){maxResult= 57},
                new ResultMax(){maxResult= 58},
                new ResultMax(){maxResult= 59}
            };
            MinuteComboBox = new List<ResultMax>(_resultMinute);
        }
        #endregion
    }
}
