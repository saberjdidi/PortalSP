using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Extensions;
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
    public class SearchNomenclatureViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Constructors
        public SearchNomenclatureViewModel()
        {

            apiService = new ApiServices();
            StatusList = GetStatus().OrderBy(t => t.name).ToList();
            ListClientAutoComplete();
            ListMaxResult();
        }
        #endregion

        #region Attributes
        public EventHandler<DialogResultNomenclatura> OnDialogClosed;
        private ObservableCollection<Nomenclatura> _nomenclature;
        private List<Nomenclatura> nomenclatureList;
        private bool isRefreshing;
        private SearchModel _searchModel;
        public List<Status> StatusList { get; set; }
        private Status _selectedStatus { get; set; }
        private int _maxResult = 50;
        #endregion

        #region Properties
        public string Code { get; set; }
        public Client Client { get; set; }
        public Status SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                    // Do whatever functionality you want...When a selectedItem is changed..
                    // write code here..
                    //Resource.Culture = new CultureInfo(_selectedStatus.name);
                }
            }
        }
        public int MaxResult
        {
            get { return _maxResult; }
            set
            {
                _maxResult = value;
                OnPropertyChanged();
            }
        }
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
        public bool IsVisible { get; set; }
        #endregion

        #region Methods
        public async void GetNomenclatureSearch()
        {
            // IsRefreshing = true;
           /* if (SelectedStatus == null)
            {
                // SelectedStatus.name = "";
                await Application.Current.MainPage.DisplayAlert("Warning", "Please Select Status", "ok");
                return;
            }*/
            
            if (Client == null && SelectedStatus == null)
            {
                _searchModel = new SearchModel
                {
                    criteria1 = Code,
                    id1 = -1,
                    id2 = MaxResult,
                    order = "asc",
                    sortedBy = "code",
                    status = "all"
                };
            }
            else if (SelectedStatus == null)
            {
                _searchModel = new SearchModel
                {
                    criteria1 = Code,
                    id1 = Client.id,
                    id2 = MaxResult,
                    order = "asc",
                    sortedBy = "code",
                    status = "all"
                };
            }
            else if (Client == null)
            {
                _searchModel = new SearchModel
                {
                    criteria1 = Code,
                    id1 = -1,
                    id2 = MaxResult,
                    order = "asc",
                    sortedBy = "code",
                    status = SelectedStatus.name
                };
            }
            else
            {
               _searchModel = new SearchModel
                {
                    criteria1 = Code,
                    id1 = Client.id,
                    id2 = MaxResult,
                    order = "asc",
                    sortedBy = "code",
                    status = SelectedStatus.name
                };
            }
            
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Nomenclatura>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/nomenclatura/search",
            res,
            _searchModel);
            nomenclatureList = (List<Nomenclatura>)response.Result;
            Nomenclatures = new ObservableCollection<Nomenclatura>(nomenclatureList);
            if (Nomenclatures.Count() == 0)
            {
                IsVisible = true;
            }
            MessagingCenter.Send(new DialogResultNomenclatura() { NomenclaturePopup = Nomenclatures }, "PopUpData");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        //Status List
        public List<Status> GetStatus()
        {
            var status = new List<Status>()
            {
                new Status(){name= "all"},
                new Status(){name= "active"},
                new Status(){name= "deactive"},
                new Status(){name= "reflex"}
            };

            return status;
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetNomenclatureSearch);
            }
        }
        public ICommand SearchCommand
        {
            get
            {
                return new Command(() =>
                {
                    GetNomenclatureSearch();
                });
            }
        }
        #endregion

        #region Autocomplete
        //Client
        private List<Client> _clientAutoComplete;
        public List<Client> ClientAutoComplete
        {
            get { return _clientAutoComplete; }
            set
            {
                _clientAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Client>> ListClientAutoComplete()
        {
            var _searchModel = new SearchModel
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
            _searchModel);
            ClientAutoComplete = (List<Client>)response.Result;
            return ClientAutoComplete;
        }
        //MaxResult
        public List<ResultMax> MaxResultComboBox { get; set; }
        public void ListMaxResult()
        {
            var _result = new List<ResultMax>()
            {
                new ResultMax(){maxResult= 50},
                new ResultMax(){maxResult= 100},
                new ResultMax(){maxResult= 250}
            };
            MaxResultComboBox = new List<ResultMax>(_result);
        }
        #endregion
    }
    #region Models
    public class DialogResultNomenclatura
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ObservableCollection<Nomenclatura> NomenclaturePopup { get; set; }
    }
    #endregion
}
