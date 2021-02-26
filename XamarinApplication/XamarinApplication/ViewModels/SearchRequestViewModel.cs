using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extended;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Resources;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
    public class SearchRequestViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Constructors
        public SearchRequestViewModel()
        {
            
            apiService = new ApiServices();
            ListPatientAutoComplete();
            ListClientAutoComplete();
            ListMaxResult();
            StatusList = GetStatus().OrderBy(t => t.name).ToList();
            DateList = GetDate().OrderBy(t => t.name).ToList();

        }
        #endregion

        #region Attributes
        public EventHandler<DialogResult> OnDialogClosed;
        private DateTime from = System.DateTime.Today;  //DateTime.Today.Date;
        private DateTime to = System.DateTime.Today;
        private ObservableCollection<Request> _requests;
        //private InfiniteScrollCollection<Request> requests;
        private List<Request> requestsList;
        private const int PageSize = 10;
        private bool isRefreshing;
        private SearchModel _searchModel;
        public List<Status> StatusList { get; set; }
        public List<Status> DateList { get; set; }
        private Status _selectedStatus { get; set; }
        private int _maxResult = 50;
        #endregion

        #region Properties
        public Patient Patient { get; set; }
        public Client Client { get; set; }
        public string RequestNum { get; set; }
        public int MaxResult
        {
            get { return _maxResult; }
            set
            {
                _maxResult = value;
                OnPropertyChanged();
            }
        }
        public Status SelectedDate { get; set; }
        public Status SelectedStatus
        {
            get { return _selectedStatus; }
            set
            {
                if (_selectedStatus != value)
                {
                    _selectedStatus = value;
                }
            }
        }
        public DateTime CheckDateFrom
        {
            get { return from; }
            set
            {
                from = value;
                OnPropertyChanged();
            }
        }
        public DateTime CheckDateTo
        {
            get { return to; }
            set
            {
                to = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Request> Requests
        {
            get { return _requests; }
            set
            {
                _requests = value;
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
        public async void GetRequestsSearch()
        {
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            // With Scrolling
            #region scrolling
            /* if (SelectedStatus == null && SelectedDate == null)
             {
                 _searchModel = new SearchModel
                 {
                     maxResult = 100,
                     order = "desc",
                     sortedBy = "request_creation_date",
                     criteria3 = "",
                     date = CheckDateFrom,
                     date1 = CheckDateTo,
                     status = "ALL"
                 };
             }
             else if (SelectedStatus == null)
             {
                 _searchModel = new SearchModel
                 {
                     maxResult = 100,
                     order = "desc",
                     sortedBy = "request_creation_date",
                     criteria3 = SelectedDate.name,
                     date = CheckDateFrom,
                     date1 = CheckDateTo,
                     status = "ALL"
                 };
             }
             else if (SelectedDate == null)
             {
                 _searchModel = new SearchModel
                 {
                     maxResult = 100,
                     order = "desc",
                     sortedBy = "request_creation_date",
                     criteria3 = "",
                     date = CheckDateFrom,
                     date1 = CheckDateTo,
                     status = SelectedStatus.name
                 };
             }
             else
             {
                  _searchModel = new SearchModel
                 {
                     maxResult = 100,
                     order = "desc",
                     sortedBy = "request_creation_date",
                     criteria3 = SelectedDate.name,
                     date = CheckDateFrom,
                     date1 = CheckDateTo,
                     status = SelectedStatus.name
                 };
             }

             var response = await apiService.PostRequest<Request>(
             "https://portalesp.smart-path.it",
             "/Portalesp",
             "/request/searchRequest?mobile=mobile",
             res,
             _searchModel);
             requestsList = (List<Request>)response.Result;
             Requests = new ObservableCollection<Request>(requestsList);
             if (Requests.Count() == 0)
             {
                 IsVisible = true;
             }
             //Requests.AddRange(requestsList);
             //OnDialogClosed?.Invoke(this, new DialogResult { Success = true, Message = "send Data", RequestsPopup = Requests });
             // IsRefreshing = false;
             */
            #endregion
            if (Patient == null && SelectedStatus == null && Client == null)
            {
                _searchModel = new SearchModel
                {
                    criteria1 = "",
                    criteria2 = "",
                    criteria3 = "",
                    criteria5 = RequestNum,
                    id1 = -1,
                    id2 = -1,
                    id3 = -1,
                    ambulatoireService = -1,
                    nomenclatureId = -1,
                    maxResult = MaxResult,
                    offset = 0,
                    order = "desc",
                    sortedBy = "request_creation_date",
                    status = "ALL"
                };
            }
            else if (Patient == null && Client == null)
            {
                _searchModel = new SearchModel
                {
                    criteria1 = "",
                    criteria2 = "",
                    criteria3 = "",
                    criteria5 = RequestNum,
                    id1 = -1,
                    id2 = -1,
                    id3 = -1,
                    ambulatoireService = -1,
                    nomenclatureId = -1,
                    maxResult = MaxResult,
                    offset = 0,
                    order = "desc",
                    sortedBy = "request_creation_date",
                    status = SelectedStatus.name
                };
            }
            else if (SelectedStatus == null && Client == null)
            {
                _searchModel = new SearchModel
                {
                    criteria1 = "",
                    criteria2 = "",
                    criteria3 = "",
                    criteria5 = RequestNum,
                    id1 = Patient.id,
                    id2 = -1,
                    id3 = -1,
                    ambulatoireService = -1,
                    nomenclatureId = -1,
                    maxResult = MaxResult,
                    offset = 0,
                    order = "desc",
                    sortedBy = "request_creation_date",
                    status = "ALL"
                };
            }
            else if (SelectedStatus == null && Patient == null)
            {
                _searchModel = new SearchModel
                {
                    criteria1 = "",
                    criteria2 = "",
                    criteria3 = "",
                    criteria5 = RequestNum,
                    id1 = -1,
                    id2 = -1,
                    id3 = Client.id,
                    ambulatoireService = -1,
                    nomenclatureId = -1,
                    maxResult = MaxResult,
                    offset = 0,
                    order = "desc",
                    sortedBy = "request_creation_date",
                    status = "ALL"
                };
            }
            else if (SelectedStatus == null)
            {
                _searchModel = new SearchModel
                {
                    criteria1 = "",
                    criteria2 = "",
                    criteria3 = "",
                    criteria5 = RequestNum,
                    id1 = Patient.id,
                    id2 = -1,
                    id3 = Client.id,
                    ambulatoireService = -1,
                    nomenclatureId = -1,
                    maxResult = MaxResult,
                    offset = 0,
                    order = "desc",
                    sortedBy = "request_creation_date",
                    status = "ALL"
                };
            }
            else if (Patient == null)
            {
                _searchModel = new SearchModel
                {
                    criteria1 = "",
                    criteria2 = "",
                    criteria3 = "",
                    criteria5 = RequestNum,
                    id1 = -1,
                    id2 = -1,
                    id3 = Client.id,
                    ambulatoireService = -1,
                    nomenclatureId = -1,
                    maxResult = MaxResult,
                    offset = 0,
                    order = "desc",
                    sortedBy = "request_creation_date",
                    status = SelectedStatus.name
                };
            }
            else if (Client == null)
            {
                _searchModel = new SearchModel
                {
                    criteria1 = "",
                    criteria2 = "",
                    criteria3 = "",
                    criteria5 = RequestNum,
                    id1 = Patient.id,
                    id2 = -1,
                    id3 = -1,
                    ambulatoireService = -1,
                    nomenclatureId = -1,
                    maxResult = MaxResult,
                    offset = 0,
                    order = "desc",
                    sortedBy = "request_creation_date",
                    status = SelectedStatus.name
                };
            }
            else
            {
                _searchModel = new SearchModel
                {
                    criteria1 = "",
                    criteria2 = "",
                    criteria3 = "",
                    criteria5 = RequestNum,
                    id1 = Patient.id,
                    id2 = -1,
                    id3 = Client.id,
                    ambulatoireService = -1,
                    nomenclatureId = -1,
                    maxResult = MaxResult,
                    offset = 0,
                    order = "desc",
                    sortedBy = "request_creation_date",
                    status = SelectedStatus.name
                };
            }

            var response = await apiService.PostRequest<Request>(
             "https://portalesp.smart-path.it",
             "/Portalesp",
             "/request/searchRequest?mobile=mobile",
             res,
             _searchModel);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            requestsList = (List<Request>)response.Result;
            Requests = new ObservableCollection<Request>(requestsList);
            MessagingCenter.Send(new DialogResult() { RequestsPopup = Requests }, "PopUpData");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        //with MaxResult

        //Status List
        public List<Status> GetStatus()
        {
            var status = new List<Status>()
            {
                new Status(){name= "ALL"},
                new Status(){name= "CH"},
                new Status(){name= "SV"},
                new Status(){name= "SE"},
                new Status(){name= "TC"},
                new Status(){name= "VL"},
                new Status(){name= "SI"},
                new Status(){name= "NS"}
            };

            return status;
        }

        //Date
        public List<Status> GetDate()
        {
            var status = new List<Status>()
            {
                new Status(){name= "request_creation_date"},
                new Status(){name= "request_check_date"},
                new Status(){name= "request_validation_date"}
            };

            return status;
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetRequestsSearch);
            }
        }
        public ICommand SearchCommand
        {
            get
            {
                return new Command(() =>
                {
                    GetRequestsSearch();
                });
            }
        }
        #endregion

        #region Autocomplete
        //Patient
        private List<Patient> _patientAutoComplete;
        public List<Patient> PatientAutoComplete
        {
            get { return _patientAutoComplete; }
            set
            {
                _patientAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Patient>> ListPatientAutoComplete()
        {
            var _search = new SearchModel
            {
                maxResult = 400,
                order = "asc",
                sortedBy = "lastName"
            };
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Patient>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/patient/search",
                 res,
                 _search);
            PatientAutoComplete = (List<Patient>)response.Result;
            return PatientAutoComplete;
        }
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
                new ResultMax(){maxResult= 10},
                new ResultMax(){maxResult= 50},
                new ResultMax(){maxResult= 100},
                new ResultMax(){maxResult= 250},
                new ResultMax(){maxResult= 500}
            };
            MaxResultComboBox = new List<ResultMax>(_result);
        }
        #endregion
    }
    #region Models
    public class Status
    {
        public string name { get; set; }
    }
    public class DialogResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ObservableCollection<Request> RequestsPopup { get; set; }
    }
    #endregion
}
