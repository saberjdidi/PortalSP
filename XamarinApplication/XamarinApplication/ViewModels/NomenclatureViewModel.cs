using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Services;
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
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
    public class NomenclatureViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Nomenclatura> _nomenclature;
        private bool isRefreshing;
        private string filter;
        private List<Nomenclatura> nomenclatureList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Nomenclatura> Nomenclatures
        {
            get { return _nomenclature; }
            set
            {
                _nomenclature = value;
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
        public NomenclatureViewModel()
        {
            apiService = new ApiServices();
            GetNomenclatures();
            //instance = this;
        }
        #endregion
        /*
        #region Sigleton
        static NomenclatureViewModel instance;
        public static NomenclatureViewModel GetInstance()
        {
            if (instance == null)
            {
                return new NomenclatureViewModel();
            }

            return instance;
        }

        public void Update(Nomenclatura icdo)
        {
            IsRefreshing = true;
            var oldIcdo = nomenclatureList
                .Where(p => p.id == icdo.id)
                .FirstOrDefault();
            oldIcdo = icdo;
            Nomenclatures = new ObservableCollection<Nomenclatura>(nomenclatureList);
            IsRefreshing = false;
        }
        public async Task Delete(Nomenclatura icdo)
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
            var response = await apiService.Delete<Nomenclatura>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/nomenclatura/delete/" + icdo.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            nomenclatureList.Remove(icdo);
            Nomenclatures = new ObservableCollection<Nomenclatura>(nomenclatureList);

            IsRefreshing = false;
        }
        #endregion
        */
        #region Methods
        public async void GetNomenclatures()
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
            var response = await apiService.GetListWithCoockie<Nomenclatura>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/nomenclatura/list?sortedBy=code&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            nomenclatureList = (List<Nomenclatura>)response.Result;
            Nomenclatures = new ObservableCollection<Nomenclatura>(nomenclatureList);
            IsRefreshing = false;
            if (Nomenclatures.Count() == 0)
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
                return new RelayCommand(GetNomenclatures);
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
                Nomenclatures = new ObservableCollection<Nomenclatura>(nomenclatureList);
            }
            else
            {
                Nomenclatures = new ObservableCollection<Nomenclatura>(
                    nomenclatureList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.descrEsameFunz.ToLower().Contains(Filter.ToLower())));
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
        public ICommand SearchPopup
        {
            get
            {
                return new Command(async () =>
                {
                    MessagingCenter.Subscribe<DialogResultNomenclatura>(this, "PopUpData", (value) =>
                    {
                        // string receivedData = value.RequestsPopup;
                        // MyLabel.Text = receivedData;
                        Nomenclatures = value.NomenclaturePopup;
                    });
                    await PopupNavigation.Instance.PushAsync(new SearchNomenclaturePage());
                });
            }
        }
        #endregion
    }
}
