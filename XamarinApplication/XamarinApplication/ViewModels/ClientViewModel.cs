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
    public class ClientViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Client> _client;
        private bool isRefreshing;
        private string filter;
        private List<Client> clientList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Client> Clients
        {
            get { return _client; }
            set
            {
                _client = value;
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
        public ClientViewModel()
        {
            apiService = new ApiServices();
            GetClients();
            instance = this;
        }
        #endregion

        #region Sigleton
        static ClientViewModel instance;
        public static ClientViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ClientViewModel();
            }

            return instance;
        }

        public void Update(Client client)
        {
            IsRefreshing = true;
            var oldclient = clientList
                .Where(p => p.id == client.id)
                .FirstOrDefault();
            oldclient = client;
            Clients = new ObservableCollection<Client>(clientList);
            IsRefreshing = false;
        }
        public async Task Delete(Client client)
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
            var response = await apiService.Delete<Client>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/client/delete/" + client.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            clientList.Remove(client);
            Clients = new ObservableCollection<Client>(clientList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetClients()
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
            var _search = new SearchModel
            {
                order = "asc",
                sortedBy = "companyName"
            };

            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            
            var response = await apiService.PostRequest<Client>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/client/searchSample",
                 res,
                 _search);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            clientList = (List<Client>)response.Result;
            Clients = new ObservableCollection<Client>(clientList);
            IsRefreshing = false;
            if (Clients.Count() == 0)
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
                return new RelayCommand(GetClients);
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
                Clients = new ObservableCollection<Client>(clientList);
            }
            else
            {
                Clients = new ObservableCollection<Client>(
                    clientList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.companyName.ToLower().Contains(Filter.ToLower())));
            }
            if (Clients.Count() == 0)
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
