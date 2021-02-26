using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class NewRequestCompilationViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewRequestCompilationViewModel()
        {
            apiService = new ApiServices();
            ListReportAutoComplete();
        }
        #endregion

        #region Properties
        private int _number = 1;
        public int Number
        {
            get { return _number; }
            set
            {
                _number = value;
                OnPropertyChanged();
            }
        }
        private Report _report = null;
        public Report Report
        {
            get { return _report; }
            set
            {
                _report = value;
                OnPropertyChanged();
            }
        }

        private bool value = false;
        public bool Value
        {
            get { return value; }
            set
            {
                this.value = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        public async void AddRequestCompilation()
        {
            Value = true;
            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Languages.Warning,
                    Languages.CheckConnection,
                    Languages.Ok);
                return;
            }
            if(Report == null)
            {
                Value = true;
                await Application.Current.MainPage.DisplayAlert("Warning", "Select Report", "ok");
                return;
            }
            var _client = new Client
            {
                id = 147,
                companyName = "A.I.E.D. MILANO SRL",
                email = "epv@ilavit.it",
                code = "A.I3GBZBEFSR6ZG",
                master = "tU0psfDbcUXwlsZ81528953254958",
                publicId = "h73lqiyTU2oz07K81535695538984",
                useReportCatalog = true
            };
            var _reportGenerator = new ReportGenerator
            {
                client = _client,
                report = Report.id.ToString()
            };
            var _requestCompilation = new AddRequestCompilation
            {
                number = Number,
                reportGenerator = _reportGenerator
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddRequestCompilation>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/reportGenerator/save",
            res,
            _requestCompilation);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Request Compilation Added");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Save
        {
            get
            {
                return new Command(() =>
                {
                    AddRequestCompilation();
                });
            }
        }
        public Command ClosePopup
        {
            get
            {
                return new Command(() =>
                {
                    Navigation.PopPopupAsync();
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
        //Report
        private List<Report> _reportAutoComplete;
        public List<Report> ReportAutoComplete
        {
            get { return _reportAutoComplete; }
            set
            {
                _reportAutoComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Report>> ListReportAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                criteria0 = "request",
                order = "asc",
                sortedBy = "name"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Report>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/report/search",
            res,
            _searchModel);
            ReportAutoComplete = (List<Report>)response.Result;
            return ReportAutoComplete;
        }
        #endregion
    }
}
