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
   public class AntecedentViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Antecedent> _antecedent;
        private bool isRefreshing;
        private string filter;
        private List<Antecedent> antecedentList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Antecedent> Antecedents
        {
            get { return _antecedent; }
            set
            {
                _antecedent = value;
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
        public AntecedentViewModel()
        {
            apiService = new ApiServices();
            GetAntecedent();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetAntecedent();
            });
        }
        #endregion

        #region Sigleton
        static AntecedentViewModel instance;
        public static AntecedentViewModel GetInstance()
        {
            if (instance == null)
            {
                return new AntecedentViewModel();
            }

            return instance;
        }

        public void Update(Antecedent antecedent)
        {
            IsRefreshing = true;
            var oldAntecedent = antecedentList
                .Where(p => p.id == antecedent.id)
                .FirstOrDefault();
            oldAntecedent = antecedent;
            Antecedents = new ObservableCollection<Antecedent>(antecedentList);
            IsRefreshing = false;
        }
        public async Task Delete(Antecedent antecedent)
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
            var response = await apiService.Delete<Antecedent>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/antecedent/delete/" + antecedent.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            antecedentList.Remove(antecedent);
            Antecedents = new ObservableCollection<Antecedent>(antecedentList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetAntecedent()
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
            var response = await apiService.GetListWithCoockie<Antecedent>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/antecedent/list?sortedBy=id&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            antecedentList = (List<Antecedent>)response.Result;
            Antecedents = new ObservableCollection<Antecedent>(antecedentList);
            IsRefreshing = false;
            if (Antecedents.Count() == 0)
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
                return new RelayCommand(GetAntecedent);
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
                Antecedents = new ObservableCollection<Antecedent>(antecedentList);
            }
            else
            {
                Antecedents = new ObservableCollection<Antecedent>(
                    antecedentList.Where(
                        l => l.description.ToLower().Contains(Filter.ToLower())));
            }

            if (Antecedents.Count() == 0)
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
