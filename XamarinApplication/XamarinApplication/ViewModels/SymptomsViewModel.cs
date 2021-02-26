using GalaSoft.MvvmLight.Command;
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
using XamarinApplication.ViewModels;

namespace XamarinApplication.ViewModels
{
   public class SymptomsViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Symptoms> _symptoms;
        private bool isRefreshing;
        private string filter;
        private List<Symptoms> symptomsList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Symptoms> Symptoms
        {
            get { return _symptoms; }
            set
            {
                _symptoms = value;
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
        #endregion

        #region Constructors
        public SymptomsViewModel()
        {
            apiService = new ApiServices();
            dialogService = new DialogService();
            GetList();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetList();
            });
        }
        #endregion

        #region Methods
        public async void GetList()
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
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.GetListWithCoockie<Symptoms>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/diagnosis/buildUncoditionalTree?checkListId=14",
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            symptomsList = (List<Symptoms>)response.Result;
            Symptoms = new ObservableCollection<Symptoms>(symptomsList);
            IsRefreshing = false;
            if (Symptoms.Count() == 0)
            {
                IsVisibleStatus = true;
            }
            else
            {
                IsVisibleStatus = false;
            }
        }
        #endregion

        #region Sigleton
        static SymptomsViewModel instance;
        public static SymptomsViewModel GetInstance()
        {
            if (instance == null)
            {
                return new SymptomsViewModel();
            }

            return instance;
        }

        public void Update(Symptoms symptoms)
        {
            IsRefreshing = true;
            var oldSymptoms = symptomsList
                .Where(p => p.data.id == symptoms.data.id)
                .FirstOrDefault();
            oldSymptoms = symptoms;
            Symptoms = new ObservableCollection<Symptoms>(symptomsList);
            IsRefreshing = false;
        }
        public async Task Delete(Symptoms symptoms)
        {
            IsRefreshing = true;

            var connection = await apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage("Error", connection.Message);
                return;
            }
            var cookie = Settings.Cookie;  //.Split(11, 33)
            var res = cookie.Substring(11, 32);
            var response = await apiService.Delete<Symptoms>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/diagnosis/deleteEsit/"+ symptoms.data.id + "?checklistId=14&id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            symptomsList.Remove(symptoms);
            Symptoms = new ObservableCollection<Symptoms>(symptomsList);
            MessagingCenter.Send((App)Application.Current, "OnSaved");
            IsRefreshing = false;
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
                Symptoms = new ObservableCollection<Symptoms>(symptomsList);
            }
            else
            {
                Symptoms = new ObservableCollection<Symptoms>(
                    symptomsList.Where(
                        l => l.data.esitDesc.ToLower().StartsWith(Filter.ToLower())));
            }
            if (Symptoms.Count() == 0)
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
        #endregion
    }
}
#region Model
public class Symptoms
{
    #region Services
    DialogService _dialogService;
    #endregion

    #region Properties
    public List<Children> children { get; set; }
    public Data data { get; set; }
    public int siblingsNumber { get; set; }
    #endregion

    #region Constructors
    public Symptoms()
    {
        _dialogService = new DialogService();
    }
    #endregion

    #region Commands
    public ICommand DeleteCommand
    {
        get
        {
            return new RelayCommand(Delete);
        }
    }
     
    async void Delete()
    {
        var response = await _dialogService.ShowConfirm(
            Languages.Confirm,
            Languages.ConfirmationDelete + "Symptoms" + " ?");
        if (!response)
        {
            return;
        }

        await SymptomsViewModel.GetInstance().Delete(this);
    }
    #endregion
}
public class Children
{
    public Data data { get; set; }
    public Data parentData { get; set; }
    public int siblingsNumber { get; set; }
}
public class Data
{
    public int id { get; set; } 
    public string esitCodi { get; set; }
    public string esitColp { get; set; }
    public string esitDefa { get; set; }
    public string esitDesc { get; set; }
    public string esitPtnm { get; set; }
    public string esitLive { get; set; }
    public bool esitEspr { get; set; }
    public bool esitSucc { get; set; }
    public string esitTest { get; set; }
    public string esitTipo { get; set; }
    public int esitOrdi { get; set; }
    public Branch esitTipoCodi { get; set; }
}
#endregion
