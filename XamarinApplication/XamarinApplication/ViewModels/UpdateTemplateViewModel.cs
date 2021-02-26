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
   public class UpdateTemplateViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private DiagnosticTemplate _diagnosticTemplate;
        #endregion

        #region Constructors
        public UpdateTemplateViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public DiagnosticTemplate DiagnosticTemplate
        {
            get { return _diagnosticTemplate; }
            set
            {
                _diagnosticTemplate = value;
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
        public async void EditTemplate()
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
            if (string.IsNullOrEmpty(DiagnosticTemplate.name) || string.IsNullOrEmpty(DiagnosticTemplate.description))
            {
                Value = true;
                return;
            }
            var template = new DiagnosticTemplate
            {
                id = DiagnosticTemplate.id,
                name = DiagnosticTemplate.name,
                description = DiagnosticTemplate.description,
                template = DiagnosticTemplate.template,
                createBy = DiagnosticTemplate.createBy
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<DiagnosticTemplate>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/diagnosticTemplate/update",
            res,
            template);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            //DiagnosticTemplateViewModel.GetInstance().Update(template);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Template Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand UpdateTemplate
        {
            get
            {
                return new Command(() =>
                {
                    EditTemplate();
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
