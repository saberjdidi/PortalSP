using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class ConfigurationUploadCSVViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Configs> _configs;
        private List<Configs> configsList;
        private bool isRefreshing = false;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Configs> Configs
        {
            get { return _configs; }
            set
            {
                _configs = value;
                OnPropertyChanged();
            }
        }
        public bool IsRefreshing
        {
            get
            {
                return isRefreshing;
            }
            set
            {
                isRefreshing = value;
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

        #region Constructors
        public ConfigurationUploadCSVViewModel()
        {
            apiService = new ApiServices();
            GetJobCron();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetJobCron();
            });
        }
        #endregion

        #region Sigleton
        static ConfigurationUploadCSVViewModel instance;
        public static ConfigurationUploadCSVViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ConfigurationUploadCSVViewModel();
            }

            return instance;
        }

         public void Update(Configs configs)
         {
             IsRefreshing = true;
             var oldConfigs = configsList
                 .Where(p => p.id == configs.id)
                 .FirstOrDefault();
             oldConfigs = configs;
             Configs = new ObservableCollection<Configs>(configsList);
             IsRefreshing = false;
         }
        public async Task DeleteJobCron(Configs configs)
        {
            IsRefreshing = true;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.Delete<Configs>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/config/delete/" + configs.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetJobCron()
        {
            IsRefreshing = true;
            var connection = await apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }
            var timestamp = DateTime.Now.ToFileTime();
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<Configs>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/config/list?sortedBy=cron&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            configsList = (List<Configs>)response.Result;
            Configs = new ObservableCollection<Configs>(configsList);
            IsRefreshing = false;
            if (Configs.Count() == 0)
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
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetJobCron);
            }
        }

        #endregion
    }
}
