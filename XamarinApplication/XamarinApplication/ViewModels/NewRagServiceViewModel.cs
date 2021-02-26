using System;
using Rg.Plugins.Popup.Extensions;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Models;
using XamarinApplication.Services;
using XamarinApplication.Helpers;

namespace XamarinApplication.ViewModels
{
   public class NewRagServiceViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        #endregion

        #region Constructor
        public NewRagServiceViewModel()
        {
            apiService = new ApiServices();
            ListReportAutoComplete();

        }
        #endregion

        #region Properties
        public string Code { get; set; }
        public string Description { get; set; }
        public Report Report { get; set; }

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
        public async void AddNewRagService()
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
            if (string.IsNullOrEmpty(Code) || string.IsNullOrEmpty(Description))
            {
                Value = true;
                return;
            }
            if (Report == null)
            {
                Value = true;
                await Application.Current.MainPage.DisplayAlert("Warning", "Report is required", "ok");
                return;
            }
            var _ragService = new AddRagService
            {
                code = Code,
                description = Description,
                report = Report
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Save<AddRagService>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/ragService/save",
            res,
            _ragService);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Rag Service Added");
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
                    AddNewRagService();
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
        //CodRL
        private List<Report> _reportComplete;
        public List<Report> ReportAutoComplete
        {
            get { return _reportComplete; }
            set
            {
                _reportComplete = value;
                OnPropertyChanged();
            }
        }
        public async Task<List<Report>> ListReportAutoComplete()
        {
            var _searchModel = new SearchModel
            {
                order = "desc",
                sortedBy = "name"
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.PostRequest<Report>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/report/listReportByClients",
            res,
            _searchModel);
            ReportAutoComplete = (List<Report>)response.Result;
            return ReportAutoComplete;
        }
        #endregion
    }
}
