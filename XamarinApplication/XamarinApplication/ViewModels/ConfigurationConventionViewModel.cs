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
    public class ConfigurationConventionViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<ConfigConvention> _configConvention;
        private List<ConfigConvention> configConventionList;
        private bool isVisible;
        #endregion

        #region Properties
        public Convention Convention { get; set; }
        public ObservableCollection<ConfigConvention> ConfigConventions
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
        #endregion 

        #region Constructors
        public ConfigurationConventionViewModel()
        {
            apiService = new ApiServices();
            GetconfigConvention();
        }
        #endregion

        #region Methods
        public async void GetconfigConvention()
        {
            var connection = await apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<ConfigConvention>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/conventionConfiguration/getConfigurationByConvention?id="+ Convention.id + "&sortedBy=client.companyName&order=asc",
                 res);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            configConventionList = (List<ConfigConvention>)response.Result;
            ConfigConventions = new ObservableCollection<ConfigConvention>(configConventionList);
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
    }
}
