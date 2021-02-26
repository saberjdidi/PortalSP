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

namespace XamarinApplication.ViewModels
{
    public class DoctorViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        DialogService dialogService;
        #endregion

        #region Attributes
        private ObservableCollection<Doctor> _doctor;
        private bool isRefreshing;
        private string filter;
        private List<Doctor> doctorList;
        bool _isVisibleStatus;
        private bool _showHide = false;
        #endregion

        #region Properties
        public ObservableCollection<Doctor> Doctors
        {
            get { return _doctor; }
            set
            {
                _doctor = value;
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
        public DoctorViewModel()
        {
            apiService = new ApiServices();
            GetDoctor();
            instance = this;

            MessagingCenter.Subscribe<App>((App)Application.Current, "OnSaved", (sender) => {
                GetDoctor();
            });
        }
        #endregion

        #region Sigleton
        static DoctorViewModel instance;
        public static DoctorViewModel GetInstance()
        {
            if (instance == null)
            {
                return new DoctorViewModel();
            }

            return instance;
        }

        public void Update(Doctor doctor)
        {
            IsRefreshing = true;
            var olddoctor = doctorList
                .Where(p => p.id == doctor.id)
                .FirstOrDefault();
            olddoctor = doctor;
            Doctors = new ObservableCollection<Doctor>(doctorList);
            IsRefreshing = false;
        }
        public async Task Delete(Doctor doctor)
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
            var response = await apiService.Delete<Doctor>(
                "https://portalesp.smart-path.it",
                "/Portalesp",
                "/doctor/delete/" + doctor.id + "?id=@id",
                res);

            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await dialogService.ShowMessage(
                    "Error",
                    response.Message);
                return;
            }

            doctorList.Remove(doctor);
            Doctors = new ObservableCollection<Doctor>(doctorList);

            IsRefreshing = false;
        }
        #endregion

        #region Methods
        public async void GetDoctor()
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
            var response = await apiService.GetListWithCoockie<Doctor>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/doctor/list?sortedBy=code&order=asc&time=" + timestamp,
                 res);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            doctorList = (List<Doctor>)response.Result;
            Doctors = new ObservableCollection<Doctor>(doctorList);
            IsRefreshing = false;
            if (Doctors.Count() == 0)
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
                return new RelayCommand(GetDoctor);
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
                Doctors = new ObservableCollection<Doctor>(doctorList);
            }
            else
            {
                Doctors = new ObservableCollection<Doctor>(
                    doctorList.Where(
                        l => l.code.ToLower().Contains(Filter.ToLower()) ||
                        l.firstName.ToLower().Contains(Filter.ToLower())));
            }
            if (Doctors.Count() == 0)
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
