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
   public class UpdateInvoiceTypeViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private InvoiceType _invoiceType;
        #endregion

        #region Constructors
        public UpdateInvoiceTypeViewModel()
        {
            apiService = new ApiServices();
        }
        #endregion

        #region Properties
        public InvoiceType InvoiceType
        {
            get { return _invoiceType; }
            set
            {
                _invoiceType = value;
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
        public async void EditInvoiceType()
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
            if (string.IsNullOrEmpty(InvoiceType.code) || string.IsNullOrEmpty(InvoiceType.description))
            {
                Value = true;
                return;
            }
            var invoiceType = new InvoiceType
            {
                id = InvoiceType.id,
                code = InvoiceType.code,
                description = InvoiceType.description,
                note = InvoiceType.note,
                valid = InvoiceType.valid
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<InvoiceType>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/electronicDocumentType/update",
            res,
            invoiceType);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            InvoiceTypeViewModel.GetInstance().Update(invoiceType);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Invoice Type Updated");
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
                    EditInvoiceType();
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
