using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;

namespace XamarinApplication.ViewModels
{
   public class RequestLogHistoricViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<RequestLogHistoric> _requestLog;
        private List<RequestLogHistoric> requestLogList;
        private bool isVisible;
        private bool isRefreshing;
        #endregion

        #region Properties
        public RequestLog RequestLog { get; set; }
        public ObservableCollection<RequestLogHistoric> RequestLogHistoric
        {
            get { return _requestLog; }
            set
            {
                _requestLog = value;
                OnPropertyChanged();
            }
        }
        public bool IsVisible
        {
            get { return isVisible; }
            set
            {
                isVisible = value;
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
        #endregion

        #region Constructors
        public RequestLogHistoricViewModel()
        {
            apiService = new ApiServices();
            GetList();
        }
        #endregion

        #region Methods
        public async void GetList()
        {
            IsRefreshing = true;
            var connection = await apiService.CheckConnection();

            if (!connection.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    connection.Message,
                    "Ok");
                await Application.Current.MainPage.Navigation.PopAsync();
                return;
            }
            var cookie = Settings.Cookie;
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<RequestLogHistoric>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/requestUploadLog/getRequestRecordsGivenFile?fileId="+ RequestLog.id,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            requestLogList = (List<RequestLogHistoric>)response.Result;
            RequestLogHistoric = new ObservableCollection<RequestLogHistoric>(requestLogList);
            IsRefreshing = false;
            if (RequestLogHistoric.Count() == 0)
            {
                IsVisible = true;
            }
            else
            {
                IsVisible = false;
            }

        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetList);
            }
        }
        #endregion
    }
}
