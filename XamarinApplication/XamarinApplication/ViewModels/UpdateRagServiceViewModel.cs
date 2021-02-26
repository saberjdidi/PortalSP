using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class UpdateRagServiceViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        public INavigation Navigation { get; set; }
        private RagService _ragService;
        #endregion

        #region Constructors
        public UpdateRagServiceViewModel()
        {
            apiService = new ApiServices();
            ListReportAutoComplete();
        }
        #endregion

        #region Properties
        public RagService RagService
        {
            get { return _ragService; }
            set
            {
                _ragService = value;
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
        public async void EditRagService()
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
            if (string.IsNullOrEmpty(RagService.code) || string.IsNullOrEmpty(RagService.description) || RagService.report == null)
            {
                Value = true;
                return;
            }
            var request = new RagService
            {
                id = RagService.id,
                code = RagService.code,
                description = RagService.description,
                report = RagService.report
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);

            var response = await apiService.Put<RagService>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/ragService/update",
            res,
            request);
            Debug.WriteLine("********responseIn ViewModel*************");
            Debug.WriteLine(response);
            if (!response.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            Value = false;
            RequestModelViewModel.GetInstance().Update(request);

            DependencyService.Get<INotification>().CreateNotification("PortalSP", "Rag Service Updated");
            await App.Current.MainPage.Navigation.PopPopupAsync(true);
        }
        #endregion

        #region Commands
        public ICommand Update
        {
            get
            {
                return new Command(() =>
                {
                    EditRagService();
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
                    //Navigation.PopAsync();
                    Debug.WriteLine("********Close*************");
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
