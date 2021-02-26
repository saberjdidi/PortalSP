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
   public class ConfigurationGlobaleConventionViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<ConfigurationConvention> _configConvention;
        private List<ConfigurationConvention> configConventionList;
        private bool isVisible;
        private bool isRefreshing;
        #endregion

        #region Properties
        public ObservableCollection<ConfigurationConvention> ConfigConventions
        {
            get { return _configConvention; }
            set
            {
                _configConvention = value;
                OnPropertyChanged();
            }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
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
        #endregion 

        #region Constructors
        public ConfigurationGlobaleConventionViewModel()
        {
            apiService = new ApiServices();
            GetconfigConvention();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetconfigConvention();
            });
        }
        #endregion

        #region Sigleton
        static ConfigurationGlobaleConventionViewModel instance;
        public static ConfigurationGlobaleConventionViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ConfigurationGlobaleConventionViewModel();
            }

            return instance;
        }

        public void Update(ConfigurationConvention config)
        {
            IsRefreshing = true;
            var oldconfig = configConventionList
                .Where(p => p.id == config.id)
                .FirstOrDefault();
            oldconfig = config;
            ConfigConventions = new ObservableCollection<ConfigurationConvention>(configConventionList);
            IsRefreshing = false;
        }
        public async Task Delete(ConfigurationConvention config)
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
            var response = await apiService.Delete<ConfigurationConvention>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/conventionGlobalConfig/delete/" + config.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            configConventionList.Remove(config);
            ConfigConventions = new ObservableCollection<ConfigurationConvention>(configConventionList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetconfigConvention()
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
            var response = await apiService.GetListWithCoockie<ConfigurationConvention>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/conventionGlobalConfig/list?sortedBy=code&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            configConventionList = (List<ConfigurationConvention>)response.Result;
            ConfigConventions = new ObservableCollection<ConfigurationConvention>(configConventionList);
            IsRefreshing = false;
            if (ConfigConventions.Count() == 0)
            {
                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }

        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetconfigConvention);
            }
        }
        #endregion
    }
}
