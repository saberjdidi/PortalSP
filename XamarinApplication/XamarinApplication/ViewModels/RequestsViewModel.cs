using GalaSoft.MvvmLight.Command;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Extended;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
    public class RequestsViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        //public string AccessToken { get; set; }
        private ObservableCollection<Request> requests;
        //private InfiniteScrollCollection<Request> requests;
        public INavigation Navigation { get; set; }
        private bool isRefreshing;
        private bool isVisible;
        private string filter;
        private List<Request> requestsList;
        private bool _isBusy;
        private const int PageSize = 10;
        private const int _maxResult = 200;
        int _offset = 0;
        public int TotalCount { get; private set; }
        #endregion

        #region Properties
        public ObservableCollection<Request> Requests
        {
            get { return requests; }
            set
            {
                requests = value;
                OnPropertyChanged();
            }
        }
        /*public InfiniteScrollCollection<Request> Requests
        {
            get { return requests; }
            set
            {
                requests = value;
                OnPropertyChanged();
            }
        }*/

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
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                this.isVisible = value;
                //this.RaisePropertyChanged("IsVisible");
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
                //Search();
            }
        }
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public RequestsViewModel()
        {
            apiService = new ApiServices();
            instance = this;
            GetRequests();

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetRequests();
            });

            /* Task.Run(async () =>
             {
                 Requests = new InfiniteScrollCollection<Request>
             {
                 OnLoadMore = async () =>
                 {
                     IsBusy = true;
                     IsRefreshing = true;
                     // load the next page
                     var page = Requests.Count / PageSize;

                     var _searchModel = new SearchModel
                     {
                         maxResult = _maxResult,
                         order = "desc",
                         sortedBy = "request_creation_date"
                     };
                     var cookie = Settings.Cookie; 
                     var res = cookie.Substring(11, 32);
                     var response = await apiService.PostRequest<Request>(
                  "https://portalesp.smart-path.it",
                  "/Portalesp",
                  "/request/searchRequest?mobile=mobile",
                  page,
                  PageSize,
                  res,
                  _searchModel);
                     requestsList = (List<Request>)response.Result;

                     IsBusy = false;
                     IsRefreshing = false;
                     return requestsList;
                 },
                 OnCanLoadMore = () =>
                 {
                     return Requests.Count < _maxResult * PageSize;
                     // return Requests.Count < TotalCount;
                 }
             };
            // GetRequests();
                 await Requests.LoadMoreAsync();
             });*/

        }
        #endregion

        #region Sigleton
        static RequestsViewModel instance;
        public static RequestsViewModel GetInstance()
        {
            if (instance == null)
            {
                return new RequestsViewModel();
            }

            return instance;
        }

        public void Update(Request request)
        {
            IsRefreshing = true;
            var oldlicence = requestsList
                .Where(p => p.id == request.id)
                .FirstOrDefault();
            oldlicence = request;
            Requests = new ObservableCollection<Request>(requestsList);
            IsRefreshing = false;
        }
        #endregion

        #region Methods
        /*public async void GetRequests()
        {
            var _searchModel = new SearchModel
            {
                order = "desc",
                maxResult = 10,
                sortedBy = "request_creation_date"
            };
            //await apiService.GetRequestsSearch(_searchModel);
            Requests = await apiService.SearchRequestsAsync(_searchModel);

            IsRefreshing = false;
        }*/
        public async void GetRequests()
        {
            IsRefreshing = true;
            //IsVisible = true;
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
            var _searchModel = new SearchModel
            {
                maxResult = 60,
                order = "desc",
                sortedBy = "request_creation_date",
                status = ""
            };

            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11,32);
            Debug.WriteLine("********cookie ViewModel*************");
            Debug.WriteLine(cookie);
            /* var response = await apiService.PostRequest<Request>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/request/searchByExample",
                 res,
                 _searchModel);
            var response = await apiService.PostRequest<Request>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/request/searchRequest?mobile=mobile",
            pageIndex: 0,
            pageSize: PageSize,
            res,
            _searchModel);*/
            var response = await apiService.PostRequest<Request>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/request/searchRequest?mobile=mobile",
            res,
            _searchModel);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                //IsVisible = true;
                IsRefreshing = true;
               await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            requestsList = (List<Request>)response.Result;
            Requests = new ObservableCollection<Request>(requestsList);
            //Requests.AddRange(requestsList);
            IsRefreshing = false;
            if (Requests.Count() == 0)
            {
                IsVisible = true;
            }
            else
            {
            IsVisible = false;
            }
        }
        public async void LoadMoreItems(Request currentItem)
        {
            int itemIndex = Requests.IndexOf(currentItem);

            _offset = Requests.Count;

            if (Requests.Count - 50 == itemIndex)
            {
                IsBusy = true;
                IsRefreshing = true;
                var _searchModel = new SearchModel
                {
                    maxResult = 10,
                    offset = _offset,
                    order = "desc",
                    sortedBy = "request_creation_date",
                    status = ""
                };
                var cookie = Settings.Cookie;  //.Split(11, 33)
                var res = cookie.Substring(11, 32);
                var response = await apiService.PostRequest<Request>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/request/searchRequest?mobile=mobile",
                res,
                _searchModel);
                if (!response.IsSuccess)
                {
                    //IsVisible = true;
                    IsRefreshing = true;
                    await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                    return;
                }
                requestsList = (List<Request>)response.Result;
                foreach (Request item in requestsList)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        IsBusy = false;
                        IsRefreshing = false;
                        Requests.Add(item);
                    });
                }
            }
        }
        #endregion

        #region Commands
        public ICommand SearchPopup
        {
            get
            {
                return new Command(async () =>
                {
                    /*var pop = new SearchRequestPage();
                    pop.OnDialogClosed += (s, args) =>
                    {
                        RequestsSearch = args.RequestsPopup;
                    };
                    await App.Current.MainPage.Navigation.PushPopupAsync(pop, true);
                    */
                    MessagingCenter.Subscribe<DialogResult>(this, "PopUpData", (value) =>
                    {
                        // string receivedData = value.RequestsPopup;
                        // MyLabel.Text = receivedData;
                        Requests = value.RequestsPopup;
                        IsRefreshing = false;
                        if (Requests.Count() == 0)
                        {
                            IsVisible = true;
                        }
                        else
                        {
                            IsVisible = false;
                        }
                    });
                    await PopupNavigation.Instance.PushAsync(new SearchRequestPage());
                });
            }
        }
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetRequests);
            }
        }
        #endregion

        #region Search use Popup
        /*
        public string SearchTxt { get; set; }
        private ObservableCollection<Request> requestsSearch;
        public List<StatusSearch> StatusList { get; set; }
        private StatusSearch _selectedStatus { get; set; }
        public StatusSearch SelectedStatus
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
        public ObservableCollection<Request> RequestsSearch
        {
            get { return requestsSearch; }
            set
            {
                requestsSearch = value;
                OnPropertyChanged();
            }
        }

        public List<StatusSearch> GetStatus()
        {
            var statusSearch = new List<StatusSearch>()
            {
                new StatusSearch(){name= ""},
                new StatusSearch(){name= "CH"},
                new StatusSearch(){name= "SV"},
                new StatusSearch(){name= "SE"},
                new StatusSearch(){name= "TC"},
                new StatusSearch(){name= "VL"},
                new StatusSearch(){name= "SI"},
                new StatusSearch(){name= "NS"}
            };

            return statusSearch;
        }

        private DateTime from = System.DateTime.Now;
        private DateTime to = System.DateTime.Now;
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
                OnPropertyChanged("to");
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
        public async void GetRequestsSearch()
        {
            var _searchModel = new SearchModel
            {
                maxResult = 200,
                order = "desc",
                sortedBy = "request_creation_date",
                date = CheckDateFrom,
                date1 = CheckDateTo,
                status = SelectedStatus.name
            };
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Request>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/request/searchRequest?mobile=mobile",
            res,
            _searchModel);
            requestsList = (List<Request>)response.Result;
            Requests.AddRange(requestsList);
            // RequestsSearch = new ObservableCollection<Request>(requestsList);
            // await PopupNavigation.Instance.PopAsync(true);
            // OnDialogClosed?.Invoke(this, new DialogResult { Success = true, Message = "send Data" });
            // await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }

        //public EventHandler<DialogResult> OnDialogClosed;
        */
        #endregion
        
    }

    /* public class StatusSearch
     {
         public string name { get; set; }
     }
     public class DialogResult
     {
         public bool Success { get; set; }
         public string Message { get; set; }
     }*/
}
