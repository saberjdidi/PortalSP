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
   public class CheckListViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<CheckList> _checkList;
        private bool isRefreshing;
        private string filter;
        private List<CheckList> checkList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<CheckList> CheckList
        {
            get { return _checkList; }
            set
            {
                _checkList = value;
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
        public CheckListViewModel()
        {
            apiService = new ApiServices();
            GetCheckList();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetCheckList();
            });
        }
        #endregion

        #region Sigleton
        static CheckListViewModel instance;
        public static CheckListViewModel GetInstance()
        {
            if (instance == null)
            {
                return new CheckListViewModel();
            }

            return instance;
        }

        public void Update(CheckList check)
        {
            IsRefreshing = true;
            var oldCheck = checkList
                .Where(p => p.id == check.id)
                .FirstOrDefault();
            oldCheck = check;
            CheckList = new ObservableCollection<CheckList>(checkList);
            IsRefreshing = false;
        }
        public async Task Delete(CheckList check)
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
            var response = await apiService.Delete<CheckList>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/diagnosis/delete/" + check.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            checkList.Remove(check);
            CheckList = new ObservableCollection<CheckList>(checkList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetCheckList()
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
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<CheckList>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/diagnosis/getCheckList?criteria=",
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            checkList = (List<CheckList>)response.Result;
            CheckList = new ObservableCollection<CheckList>(checkList);
            IsRefreshing = false;
            if (CheckList.Count() == 0)
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
                return new RelayCommand(GetCheckList);
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
                CheckList = new ObservableCollection<CheckList>(checkList);
            }
            else
            {
                CheckList = new ObservableCollection<CheckList>(
                    checkList.Where(
                        l => l.chlsCodi.ToLower().StartsWith(Filter.ToLower()) ||
                        l.chlsDesc.ToLower().StartsWith(Filter.ToLower())));
            }
            if (CheckList.Count() == 0)
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
