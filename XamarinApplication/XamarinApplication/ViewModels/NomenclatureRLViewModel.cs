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
    public class NomenclatureRLViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<NomenclatureRL> _nomenclatureRL;
        private bool isRefreshing;
        private string filter;
        private List<NomenclatureRL> nomenclatureList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<NomenclatureRL> NomenclatureRL
        {
            get { return _nomenclatureRL; }
            set
            {
                _nomenclatureRL = value;
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
        public NomenclatureRLViewModel()
        {
            apiService = new ApiServices();
            GetNomenclatureRL();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetNomenclatureRL();
            });
        }
        #endregion

        #region Sigleton
        static NomenclatureRLViewModel instance;
        public static NomenclatureRLViewModel GetInstance()
        {
            if (instance == null)
            {
                return new NomenclatureRLViewModel();
            }

            return instance;
        }

        public void Update(NomenclatureRL nomenclature)
        {
            IsRefreshing = true;
            var oldNomenclature = nomenclatureList
                .Where(p => p.id == nomenclature.id)
                .FirstOrDefault();
            oldNomenclature = nomenclature;
            NomenclatureRL = new ObservableCollection<NomenclatureRL>(nomenclatureList);
            IsRefreshing = false;
        }
        public async Task Delete(NomenclatureRL nomenclature)
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
            var response = await apiService.Delete<NomenclatureRL>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/nomenclatureRL/delete/" + nomenclature.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            nomenclatureList.Remove(nomenclature);
            NomenclatureRL = new ObservableCollection<NomenclatureRL>(nomenclatureList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetNomenclatureRL()
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
            var response = await apiService.GetListWithCoockie<NomenclatureRL>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/nomenclatureRL/list?sortedBy=code&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            nomenclatureList = (List<NomenclatureRL>)response.Result;
            NomenclatureRL = new ObservableCollection<NomenclatureRL>(nomenclatureList);
            IsRefreshing = false;
            if (NomenclatureRL.Count() == 0)
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
                return new RelayCommand(GetNomenclatureRL);
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
                NomenclatureRL = new ObservableCollection<NomenclatureRL>(nomenclatureList);
            }
            else
            {
                NomenclatureRL = new ObservableCollection<NomenclatureRL>(
                    nomenclatureList.Where(
                        l => l.code.ToLower().StartsWith(Filter.ToLower()) ||
                        l.description.ToLower().StartsWith(Filter.ToLower())));

                if (NomenclatureRL.Count() == 0)
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
