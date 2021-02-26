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
   public class NewAccountViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewAccountViewModel()
        {
            apiService = new ApiServices();
            ListSex = GetSex().OrderBy(t => t.Value).ToList();
        }
        #endregion

        #region Properties
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
        public string Code { get; set; }
        public string Description { get; set; }
        #endregion

        #region Methods
        public async void AddRequestCatatog()
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
            if (string.IsNullOrEmpty(Description) || string.IsNullOrEmpty(Code))
            {
                Value = true;
                return;
            }
            var requestCatalog = new AddRequestCatalog
            {
                code = Code,
                description = Description
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddRequestCatalog>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/requestCatalog/save",
            res,
            requestCatalog);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "User Added");
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
                    AddRequestCatatog();
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

        #region sex
        //***********Sex**********
        public List<Language> ListSex { get; set; }
        private Language _selectedSex { get; set; }
        public Language SelectedSex
        {
            get { return _selectedSex; }
            set
            {
                if (_selectedSex != value)
                {
                    _selectedSex = value;
                    OnPropertyChanged();
                }
            }
        }
        public List<Language> GetSex()
        {
            var languages = new List<Language>()
            {
                new Language(){Key =  "M", Value= "Male"},
                new Language(){Key =  "F", Value= "Female"}
            };

            return languages;
        }
        #endregion
    }
}
