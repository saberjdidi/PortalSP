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
   public class PaymentViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Payment> _payment;
        private bool isRefreshing;
        private string filter;
        private List<Payment> paymentList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Payment> Payments
        {
            get { return _payment; }
            set
            {
                _payment = value;
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
        public PaymentViewModel()
        {
            apiService = new ApiServices();
            GetPayments();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetPayments();
            });
        }
        #endregion

        #region Sigleton
        static PaymentViewModel instance;
        public static PaymentViewModel GetInstance()
        {
            if (instance == null)
            {
                return new PaymentViewModel();
            }

            return instance;
        }

        public void Update(Payment payment)
        {
            IsRefreshing = true;
            var oldpayment = paymentList
                .Where(p => p.id == payment.id)
                .FirstOrDefault();
            oldpayment = payment;
            Payments = new ObservableCollection<Payment>(paymentList);
            IsRefreshing = false;
        }
        public async Task Delete(Payment payment)
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
            var response = await apiService.Delete<Payment>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/payment/delete/" + payment.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            paymentList.Remove(payment);
            Payments = new ObservableCollection<Payment>(paymentList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetPayments()
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
            var response = await apiService.GetListWithCoockie<Payment>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/payment/list?sortedBy=description&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            paymentList = (List<Payment>)response.Result;
            Payments = new ObservableCollection<Payment>(paymentList);
            IsRefreshing = false;
            if (Payments.Count() == 0)
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
                return new RelayCommand(GetPayments);
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
                Payments = new ObservableCollection<Payment>(paymentList);
            }
            else
            {
                Payments = new ObservableCollection<Payment>(
                    paymentList.Where(
                        l => l.code.ToLower().StartsWith(Filter.ToLower()) ||
                        l.description.ToLower().StartsWith(Filter.ToLower())));

                if (Payments.Count() == 0)
                {
                    IsVisibleStatus = true;
                }
                else
                {
                    IsVisibleStatus = false;
                }
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
