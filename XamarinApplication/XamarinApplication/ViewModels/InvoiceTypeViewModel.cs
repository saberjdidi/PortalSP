using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class InvoiceTypeViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<InvoiceType> _invoiceType;
        private bool isRefreshing;
        private string filter;
        private List<InvoiceType> invoiceTypeList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<InvoiceType> InvoiceType
        {
            get { return _invoiceType; }
            set
            {
                _invoiceType = value;
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
        public string Filter
        {
            get { return filter; }
            set
            {
                filter = value;
                OnPropertyChanged();
                Search();
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
        public bool ShowHide
        {
            get => _showHide;
            set
            {
                _showHide = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public InvoiceTypeViewModel()
        {
            apiService = new ApiServices();
            GetInvoiceType();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetInvoiceType();
            });
        }
        #endregion

        #region Sigleton
        static InvoiceTypeViewModel instance;
        public static InvoiceTypeViewModel GetInstance()
        {
            if (instance == null)
            {
                return new InvoiceTypeViewModel();
            }

            return instance;
        }

        public void Update(InvoiceType invoice)
        {
            IsRefreshing = true;
            var oldinvoice = invoiceTypeList
                .Where(p => p.id == invoice.id)
                .FirstOrDefault();
            oldinvoice = invoice;
            InvoiceType = new ObservableCollection<InvoiceType>(invoiceTypeList);
            IsRefreshing = false;
        }
        public async Task Delete(InvoiceType invoice)
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
            var response = await apiService.Delete<InvoiceType>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/electronicDocumentType/delete/" + invoice.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            invoiceTypeList.Remove(invoice);
            InvoiceType = new ObservableCollection<InvoiceType>(invoiceTypeList);

            IsRefreshing = false;
        }
        public async Task ChangeChecked(InvoiceType invoice)
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
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
            var client = new HttpClient(handler);
            var url = "https://portalesp.smart-path.it/Portalesp/electronicDocumentType/setDefault?id=" + invoice.id;
            client.BaseAddress = new Uri(url);
            cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
            //var request = new HttpRequestMessage(HttpMethod.Post, url);
            //var response = await client.SendAsync(request);
            var response = await client.PostAsync(url, null);
            var result = await response.Content.ReadAsStringAsync();

            InvoiceType = new ObservableCollection<InvoiceType>(invoiceTypeList);
            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetInvoiceType()
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
            var response = await apiService.GetListWithCoockie<InvoiceType>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/electronicDocumentType/list?sortedBy=description&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            invoiceTypeList = (List<InvoiceType>)response.Result;
            InvoiceType = new ObservableCollection<InvoiceType>(invoiceTypeList);
            IsRefreshing = false;
            if (InvoiceType.Count() == 0)
            {
                IsVisibleStatus = true;
            }
            else
            {
                IsVisibleStatus = false;
            }
        }
        public async void DataAfterChecked()
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
            var timestamp = DateTime.Now.ToFileTime();
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<InvoiceType>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/electronicDocumentType/list?sortedBy=description&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            invoiceTypeList = (List<InvoiceType>)response.Result;
            InvoiceType = new ObservableCollection<InvoiceType>(invoiceTypeList);
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetInvoiceType);
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(Search);
            }
        }

        private void Search()
        {
            if (string.IsNullOrEmpty(Filter))
            {
                InvoiceType = new ObservableCollection<InvoiceType>(invoiceTypeList);
            }
            else
            {
                InvoiceType = new ObservableCollection<InvoiceType>(
                    invoiceTypeList.Where(
                        l => l.code.ToLower().StartsWith(Filter.ToLower()) ||
                        l.description.ToLower().StartsWith(Filter.ToLower())));
            }
            if (InvoiceType.Count() == 0)
            {
                IsVisibleStatus = true;
            }
            else
            {
                IsVisibleStatus = false;
            }
        }
        public ICommand OpenSearchBar
        {
            get
            {
                return new Command(() =>
                {
                    if (ShowHide == false)
                    {
                        ShowHide = true;
                    }
                    else
                    {
                        ShowHide = false;
                    }
                });
            }
        }
        #endregion
    }
}
