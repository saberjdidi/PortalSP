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
    public class EventPatientViewModel : BaseViewModel
    {
        #region Services
        private ApiServices apiService;
        #endregion

        #region Attributes
        private ObservableCollection<Event> events;
        private List<Event> eventsList;
        private bool isVisible;
        private bool isRefreshing;
        #endregion

        #region Properties
        public Patient Patient { get; set; }
        public ObservableCollection<Event> Events
        {
            get { return events; }
            set
            {
                events = value;
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
        public EventPatientViewModel()
        {
            apiService = new ApiServices();
            GetEvents();
        }
        #endregion

        #region Methods
        public async void GetEvents()
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
            var response = await apiService.GetList<Event>(
                 "https://portalesp.smart-path.it",
                 "/Portalesp",
                 "/patient/getEventsByPatient?patientId=" + Patient.id);
            if (!response.IsSuccess)
            {
                IsRefreshing = false;
                await Application.Current.MainPage.DisplayAlert("Error", response.Message, "ok");
                return;
            }
            eventsList = (List<Event>)response.Result;
            Events = new ObservableCollection<Event>(eventsList);
            IsRefreshing = false;
            if (Events.Count() == 0)
            {
                IsVisible = true;
            } else {
                IsVisible = false;
            }

        }
        #endregion

        #region Commands
        public ICommand RefreshCommand
        {
            get
            {
                return new RelayCommand(GetEvents);
            }
        }
        #endregion
    }
}
