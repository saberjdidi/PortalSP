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
   public class UpdateServiceDocumentViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private ServiceDocument _serviceDocument;
        #endregion

        #region Constructors
        public UpdateServiceDocumentViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public ServiceDocument ServiceDocument
        {
            get { return _serviceDocument; }
            set
            {
                _serviceDocument = value;
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
        public async void EditServiceDocument()
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
            if (string.IsNullOrEmpty(ServiceDocument.code) || string.IsNullOrEmpty(ServiceDocument.name) || string.IsNullOrEmpty(ServiceDocument.url))
            {
                Value = true;
                return;
            }
            var serviceDocument = new ServiceDocument
            {
                id = ServiceDocument.id,
                code = ServiceDocument.code,
                name = ServiceDocument.name,
                url = ServiceDocument.url,
                isActive = ServiceDocument.isActive
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<ServiceDocument>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/serviceDocument/saveServiceDoc",
            res,
            serviceDocument);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            ServiceDocumentViewModel.GetInstance().Update(serviceDocument);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Service Document Updated");
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
                    EditServiceDocument();
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
