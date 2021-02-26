using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
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
   public class SearchAttachmentViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Constructors
        public SearchAttachmentViewModel()
        {
            apiService = new ApiServices();
            ListPatientAutoComplete();
            ListClientAutoComplete();
            ListMaxResult();
            StatusList = GetStatus().OrderBy(t => t.name).ToList();
        }
        #endregion

        #region Attributes
        public EventHandler<DialogResultAttachment> OnDialogClosed;
        private ObservableCollection<Attachment> attachments;
        private List<Attachment> attachmentsList;
        private bool isRefreshing;
        public List<Status> StatusList { get; set; }
        private Status _selectedStatus { get; set; }
        private SearchAttachment _search;
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
        public ObservableCollection<Attachment> Attachments
        {
            get { return attachments; }
            set
            {
                attachments = value;
                OnPropertyChanged();
            }
        }
        private User _user = null;
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }
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
        public async void GetattachmentSearch()
        {
            var Username = Settings.Username;
            User = JsonConvert.DeserializeObject<User>(Username);

            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            /* if (SelectedStatus == null)
             {
                 await Application.Current.MainPage.DisplayAlert("Warning", "Please Select Status", "ok");
                 return;
             }*/
            if (Patient == null && SelectedStatus == null && Client == null)
            {
                _search = new SearchAttachment
                {
                    criteria4 = "",
                    criteria5 = RequestNum,
                    date = null,
                    date1 = null,
                    id1 = -1,
                    id3 = -1,
                    order = "desc",
                    sortedBy = "request_creation_date",
                    status = "ALL",
                    offset = 0,
                    maxResult = MaxResult
                };
            } else if (Patient == null && Client == null)
            {
                _search = new SearchAttachment
                {
                    criteria4 = "",
                    criteria5 = RequestNum,
                    date = null,
                    date1 = null,
                    id1 = -1,
                    id3 = -1,
                    order = "desc",
                    sortedBy = "request_creation_date",
                    status = SelectedStatus.name,
                    offset = 0,
                    maxResult = MaxResult
                };
            }
                else if (SelectedStatus == null && Client == null)
                {
                    _search = new SearchAttachment
                    {
                        criteria4 = "",
                        criteria5 = RequestNum,
                        date = null,
                        date1 = null,
                        id1 = Patient.id,
                        id3 = -1,
                        order = "desc",
                        sortedBy = "request_creation_date",
                        status = "ALL",
                        offset = 0,
                        maxResult = MaxResult
                    };
                }
                else if (SelectedStatus == null && Patient == null)
                {
                    _search = new SearchAttachment
                    {
                        criteria4 = "",
                        criteria5 = RequestNum,
                        date = null,
                        date1 = null,
                        id1 = -1,
                        id3 = Client.id,
                        order = "desc",
                        sortedBy = "request_creation_date",
                        status = "ALL",
                        offset = 0,
                        maxResult = MaxResult
                    };
                }
                else if (SelectedStatus == null)
                {
                    _search = new SearchAttachment
                    {
                        criteria4 = "",
                        criteria5 = RequestNum,
                        date = null,
                        date1 = null,
                        id1 = Patient.id,
                        id3 = Client.id,
                        order = "desc",
                        sortedBy = "request_creation_date",
                        status = "ALL",
                        offset = 0,
                        maxResult = MaxResult
                    };
                }
                else if (Patient == null)
                {
                    _search = new SearchAttachment
                    {
                        criteria4 = "",
                        criteria5 = RequestNum,
                        date = null,
                        date1 = null,
                        id1 = -1,
                        id3 = Client.id,
                        order = "desc",
                        sortedBy = "request_creation_date",
                        status = SelectedStatus.name,
                        offset = 0,
                        maxResult = MaxResult
                    };
                }
                else if (Client == null)
                {
                    _search = new SearchAttachment
                    {
                        criteria4 = "",
                        criteria5 = RequestNum,
                        date = null,
                        date1 = null,
                        id1 = Patient.id,
                        id3 = -1,
                        order = "desc",
                        sortedBy = "request_creation_date",
                        status = SelectedStatus.name,
                        offset = 0,
                        maxResult = MaxResult
                    };
                }
                else
                {
                    _search = new SearchAttachment
                    {
                        criteria4 = "",
                        criteria5 = RequestNum,
                        date = null,
                        date1 = null,
                        id1 = Patient.id,
                        id3 = Client.id,
                        order = "desc",
                        sortedBy = "request_creation_date",
                        status = SelectedStatus.name,
                        offset = 0,
                        maxResult = MaxResult
                    };
                }

                var response = await apiService.PostAttachment<Attachment>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/referti/search",
                 res,
                 _search);
                if (!response.IsSuccess)
                {
                    IsRefreshing = false;
                    await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                    return;
                }
                attachmentsList = (List<Attachment>)response.Result;

            Attachments = new ObservableCollection<Attachment>(attachmentsList);
            if (Attachments.Count() == 0)
            {
                IsVisible = true;
            }
            MessagingCenter.Send(new DialogResultAttachment() { AttachmentPopup = Attachments }, "PopUpData");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        //Status List
        public List<Status> GetStatus()
        {
               var status = new List<Status>()
            {
                new Status(){name= "ALL"},
                //new Status(){name= "CH"},
                //new Status(){name= "SV"},
                //new Status(){name= "SE"},
                new Status(){name= "VL"},
                new Status(){name= "SI"}
            };
                return status;
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetattachmentSearch);
            }
        }
        public ICommand SearchCommand
        {
            get
            {
                return new Command(() =>
                {
                    GetattachmentSearch();
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
    public class DialogResultAttachment
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ObservableCollection<Attachment> AttachmentPopup { get; set; }
    }
    public class ResultMax
    {
        public int maxResult { get; set; }

    }
    #endregion
}
