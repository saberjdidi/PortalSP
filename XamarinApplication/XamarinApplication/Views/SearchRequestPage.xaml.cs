using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchRequestPage : PopupPage, INotifyPropertyChanged
    {
        public SearchRequestPage()
        {
            BindingContext = new SearchRequestViewModel();
            //BindingContext = new RequestsViewModel(Navigation);
            InitializeComponent();
            //apiService = new ApiServices();
           // StatusList = GetStatus().OrderBy(t => t.name).ToList();
        }
        //add
        /*
        public List<Status> StatusList { get; set; }
        private Status _selectedStatus { get; set; }
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
        public List<Status> GetStatus()
        {
            var status = new List<Status>()
            {
                new Status(){name= ""},
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

        #region Services
        private ApiServices apiService;
        #endregion
        private DateTime from = System.DateTime.Now;  //DateTime.Today.Date;
        private DateTime to = System.DateTime.Now;
        private ObservableCollection<Request> requests;
        private List<Request> requestsList;
        private const int PageSize = 10;
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

        public ObservableCollection<Request> Requests
        {
            get { return requests; }
            set
            {
                requests = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //end





        public EventHandler<DialogResult> OnDialogClosed;

        private async void Close_Popup_Search(object sender, EventArgs e)
        {
            var _searchModel = new SearchModel
            {
                maxResult = 1000,
                order = "desc",
                sortedBy = "request_creation_date",
                date = CheckDateFrom,
                date1 = CheckDateTo,
                status = StatusPicker.ToString()
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
            Requests = new ObservableCollection<Request>(requestsList);
            // await PopupNavigation.Instance.PopAsync(true);
            OnDialogClosed?.Invoke(this, new DialogResult { Success = true, Message = "send Data", RequestsPopup = Requests });
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        */
    }

   /* public class Status
    {
        public string name { get; set; }
    }
    public class DialogResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ObservableCollection<Request> RequestsPopup { get; set; }
    }*/
}