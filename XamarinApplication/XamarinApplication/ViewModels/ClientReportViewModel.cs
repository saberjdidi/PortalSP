using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
   public class ClientReportViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<ClientReport> clientReport;
        private List<ClientReport> clientReportsList;
        private bool isVisible;
        private bool isRefreshing;
        #endregion

        #region Properties
        public int IdClient { get; set; }
        public Client Client { get; set; }
        public ObservableCollection<ClientReport> ClientReports
        {
            get { return clientReport; }
            set
            {
                clientReport = value;
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
        public ClientReportViewModel()
        {
            apiService = new ApiServices();
            GetClientId();
            GetClientReports();
        }
        #endregion

        #region Methods
        public async void GetClientId()
        {

            MessagingCenter.Subscribe<PassIdPatient>(this, "ClientId", async (value) =>
            {
                IdClient = value.idPatient;
                Debug.WriteLine("********Id of client*************");
                Debug.WriteLine(IdClient);
            });
        }
        public async void GetClientReports()
        {
            IsRefreshing = true;
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
            var response = await apiService.GetListWithCoockie<ClientReport>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/reportGenerator/listReportsOfClient?sortedBy=code&order1=asc&clientId="+ IdClient + "&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            clientReportsList = (List<ClientReport>)response.Result;
            ClientReports = new ObservableCollection<ClientReport>(clientReportsList);
            IsRefreshing = false;
            if (ClientReports.Count() == 0)
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
                return new RelayCommand(GetClientReports);
            }
        }
        #endregion
    }
}
