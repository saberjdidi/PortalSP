﻿using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
   public class RequestADMINViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<Request> requests;
        private bool isRefreshing;
        private string filter;
        private List<Request> requestsList;
        bool _isVisibleStatus;
        private bool _showHide = false;
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
        public RequestADMINViewModel()
        {
            apiService = new ApiServices();
            GetAttachments();

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetAttachments();
            });
        }
        #endregion

        #region Methods
        public async void GetAttachments()
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
            var _searchModel = new SearchModel
            {
                criteria0 = "",
                criteria1 = "",
                criteria2 = "",
                criteria3 = "",
                criteria4 = "",
                criteria5 = "",
                id1 = -1,
                id2 = -1,
                id3 = -1,
                downloadStatus = 0,
                ambulatoireService = -1,
                nomenclatureId = -1,
                maxResult = 50,
                offset = 0,
                order = "desc",
                sortedBy = "request_creation_date",
                status = ""
            };

            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            Debug.WriteLine("********cookie ViewModel*************");
            Debug.WriteLine(cookie);
            var response = await apiService.PostRequest<Request>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/request/searchRequest?mobile=mobile",
            res,
            _searchModel);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
               // await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            requestsList = (List<Request>)response.Result;
            Requests = new ObservableCollection<Request>(requestsList);
            IsRefreshing = false;
            if (Requests.Count() == 0)
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
                return new RelayCommand(GetAttachments);
            }
        }
        public ICommand SearchPopup
        {
            get
            {
                return new Command(async () =>
                {
                    MessagingCenter.Subscribe<DialogResult>(this, "PopUpData", (value) =>
                    {
                        // string receivedData = value.RequestsPopup;
                        // MyLabel.Text = receivedData;
                        Requests = value.RequestsPopup;
                        if (Requests.Count() == 0)
                        {
                            IsVisibleStatus = true;
                        }
                        else
                        {
                            IsVisibleStatus = false;
                        }
                    });
                    await PopupNavigation.Instance.PushAsync(new SearchRequestPage());
                });
            }
        }
        public ICommand GenerateServiceReport
        {
            get
            {
                return new Command(async () =>
                {
                    IsRefreshing = true;
                    var cookie = Settings.Cookie;
                    var res = cookie.Substring(11, 32);
                    var dateNow = DateTime.Now.ToString("dd-MM-yyyy");

                    var _searchModel = new SearchModel
                    {
                        criteria0 = "",
                        criteria1 = "",
                        criteria2 = "",
                        criteria3 = "",
                        criteria4 = "",
                        criteria5 = "",
                        id1 = -1,
                        id2 = -1,
                        id3 = -1,
                        downloadStatus = 0,
                        ambulatoireService = -1,
                        nomenclatureId = -1,
                        maxResult = -1,
                        offset = 0,
                        order = "desc",
                        sortedBy = "request_creation_date",
                        status = ""
                    };
                    var request = JsonConvert.SerializeObject(_searchModel);
                    Debug.WriteLine("********request*************");
                    Debug.WriteLine(request);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var cookieContainer = new CookieContainer();
                    var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                    var client = new HttpClient(handler);
                    //client.Timeout = TimeSpan.FromSeconds(200); // this is double the default
                    var url = "https://portalesp.smart-path.it/Portalesp/request/generateServiceReport";
                    Debug.WriteLine("********url*************");
                    Debug.WriteLine(url);
                    client.BaseAddress = new Uri(url);
                    cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
                    var response = await client.PostAsync(url, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        IsRefreshing = false;
                        await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                        return;
                    }

                    var result = await response.Content.ReadAsStreamAsync();
                    Debug.WriteLine("********resultStream*************");
                    Debug.WriteLine(result);
                    IsRefreshing = false;
                    using (var streamReader = new MemoryStream())
                    {
                        result.CopyTo(streamReader);
                        byte[] bytes = streamReader.ToArray();
                        MemoryStream stream = new MemoryStream(bytes);
                        Debug.WriteLine("********stream*************");
                        Debug.WriteLine(stream);
                        if (stream == null)
                        {
                            await Application.Current.MainPage.DisplayAlert("Warning", "Data is Empty", "ok");
                            return;
                        }
                        
                        await DependencyService.Get<ISave>().SaveAndView("Request_service-" + dateNow + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", stream);
                    }
                });
            }
        }
        #endregion
    }
}
