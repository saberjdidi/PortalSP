using GalaSoft.MvvmLight.Command;
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
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
   public class ConventionViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Convention> _convention;
        private bool isRefreshing;
        private string filter;
        private List<Convention> conventionList;
        bool _isVisibleStatus;
        bool _deleteConvention = false;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Convention> Conventions
        {
            get { return _convention; }
            set
            {
                _convention = value;
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
                Search();
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
        public bool DeleteConvention
        {
            get { return _deleteConvention; }
            set
            {
                _deleteConvention = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Constructors
        public ConventionViewModel()
        {
            apiService = new ApiServices();
            GetConventions();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetConventions();
            });
        }
        #endregion

        #region Sigleton
        static ConventionViewModel instance;
        public static ConventionViewModel GetInstance()
        {
            if (instance == null)
            {
                return new ConventionViewModel();
            }

            return instance;
        }

        public void Update(Convention convention)
        {
            IsRefreshing = true;
            var oldconvention = conventionList
                .Where(p => p.id == convention.id)
                .FirstOrDefault();
            oldconvention = convention;
            Conventions = new ObservableCollection<Convention>(conventionList);
            IsRefreshing = false;
        }
        public async Task Delete(Convention convention)
        {
            IsRefreshing = true;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }
            var _convention = new Convention
            {
                id = convention.id,
                socialReason = convention.socialReason,
                status = convention.status,
                startValidation = convention.startValidation,
                endValidation = convention.endValidation,
                deactivationDate = convention.deactivationDate,
                tva = convention.tva,
                discount = convention.discount,
                collaboratorNumber = convention.collaboratorNumber
            };
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.Save<Convention>(
            "https://portalesp.smart-path.it",
            "/Portalesp",
            "/convention/deactivate",
            res,
            _convention);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            conventionList.Remove(convention);
            Conventions = new ObservableCollection<Convention>(conventionList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetConventions()
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
            var timestamp = DateTime.Now.ToFileTime();
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<Convention>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/convention/list?sortedBy=socialReason&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            conventionList = (List<Convention>)response.Result;
            Conventions = new ObservableCollection<Convention>(conventionList);
            IsRefreshing = false;
            if (Conventions.Count() == 0)
            {
                IsVisibleStatus = true;
            }
            else
            {
                IsVisibleStatus = false;
            }

            var ab = Conventions.Select(c => c.status.name.Equals("AC")).FirstOrDefault();
             if (ab)
             {
                 DeleteConvention = true;
             }
             else
             {
                 DeleteConvention = false;
             }
            /*var cn = Conventions.FirstOrDefault(l => l.status.name.Equals("AC")).ToString();
             public bool ParseBool(string input)
             {
                 switch (input.ToLower())
                 {
                     case "y":
                     case "yes":
                         return true;
                     default:
                         return false;
                 }
             }

             if (ParseBool(Conventions.Where(l => l.status.name.Equals("AC")).ToString()))
             {
                 DeleteConvention = true;
             }
             string con = conventionList.Where(w => w.status.name == "AC").ToString();
               if (bool.Parse(con))
               {
                   DeleteConvention = true;
               }
               else
               {
                   DeleteConvention = false;
               }
               CollectionView collectionView = new CollectionView { ItemsSource = conventionList.Where(w => w.status.name == "AC") };
               */
        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetConventions);
            }
        }

        public ICommand SearchCommand
        {
            get
            {
                return new RelayCommand(Search);
            }
        }

        private void Search()
        {
            if (string.IsNullOrEmpty(Filter))
            {
                Conventions = new ObservableCollection<Convention>(conventionList);
            }
            else
            {
                Conventions = new ObservableCollection<Convention>(
                    conventionList.Where(
                        l => l.socialReason.ToLower().Contains(Filter.ToLower())));

            }
            if (Conventions.Count() == 0)
            {
                IsVisibleStatus = true;
            }
            else
            {
                IsVisibleStatus = false;
            }
        }
        public ICommand OpenSearchBar
        {
            get
            {
                return new Command(() =>
                {
                    if (ShowHide == false)
                    {
                        ShowHide = true;
                    }
                    else
                    {
                        ShowHide = false;
                    }
                });
            }
        }
        public ICommand DownloadExcel
        {
            get
            {
                return new Command(async() =>
                {
                    IsRefreshing = true;
                    var cookie = Settings.Cookie;
                    var res = cookie.Substring(11, 32);
                    var dateNow = DateTime.Now.ToString("dd-MM-yyyy");
                    var _convention = new ConventionList
                    {
                        conventions = conventionList
                    };

                    var request = JsonConvert.SerializeObject(_convention);
                    Debug.WriteLine("********request*************");
                    Debug.WriteLine(request);
                    var content = new StringContent(request, Encoding.UTF8, "application/json");

                    var cookieContainer = new CookieContainer();
                    var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                    var client = new HttpClient(handler);
                    var url = "https://portalesp.smart-path.it/Portalesp/convention/exportExcel";
                    Debug.WriteLine("********url*************");
                    Debug.WriteLine(url);
                    //DependencyService.Get<INotification>().CreateNotification("Download Excel", "Please wait a few seconds !");
                    client.BaseAddress = new Uri(url);
                    cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
                    var response = await client.PostAsync(url, content);
                    if (!response.IsSuccessStatusCode)
                    {
                        IsRefreshing = false;
                        await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                        return;
                    }
                   // PopuPage page1 = new PopuPage();
                   // await PopupNavigation.Instance.PushAsync(page1);
                   // await Task.Delay(2000);
                    var result = await response.Content.ReadAsStreamAsync();
                    Debug.WriteLine("********result*************");
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

                        await DependencyService.Get<ISave>().SaveAndView("Conventions" + dateNow + ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", stream);
                    }
                });
            }
        }
        #endregion
    }
}
