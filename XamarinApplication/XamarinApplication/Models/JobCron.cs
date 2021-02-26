using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using XamarinApplication.Helpers;
using XamarinApplication.Services;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Models
{
    public class JobCron
    {
        public List<Configs> configs { get; set; }
        public List<Addresses> addresses { get; set; } 
    }

    public class Configs
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string code { get; set; }
        public string cron { get; set; }
        #endregion

        #region Constructors
        public Configs()
        {
            dialogService = new DialogService();
        }
        #endregion

        #region Commands
        //Job Cron
        public ICommand DeleteJobCron
        {
            get
            {
                return new RelayCommand(DeleteCron);
            }
        }

        async void DeleteCron()
        {
            var response = await dialogService.ShowConfirm(
                "Confirm",
                Languages.ConfirmationDelete + " Job Cron ?");
            if (!response)
            {
                return;
            }

            await JobCronExpressionViewModel.GetInstance().DeleteJobCron(this);
        }
        //Posted Report
        public ICommand DeleteJobCronPostedReport
        {
            get
            {
                return new RelayCommand(DeleteCronPostedReport);
            }
        }

        async void DeleteCronPostedReport()
        {
            var response = await dialogService.ShowConfirm(
                "Confirm",
                Languages.ConfirmationDelete + " Job Cron ?");
            if (!response)
            {
                return;
            }

            await ConfigurationPostedReportViewModel.GetInstance().DeleteJobCron(this);
        }
        //Configuration Upload CSV
        public ICommand DeleteConfigurationUploadCSV
        {
            get
            {
                return new RelayCommand(DeleteUploadCSV);
            }
        }

        async void DeleteUploadCSV()
        {
            var response = await dialogService.ShowConfirm(
                "Confirm",
                Languages.ConfirmationDelete + " Job Cron ?");
            if (!response)
            {
                return;
            }

            await ConfigurationUploadCSVViewModel.GetInstance().DeleteJobCron(this);
        }
        #endregion
    }
    public class Addresses
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string code { get; set; }
        public string addressMail { get; set; } 
        #endregion

        #region Constructors
        public Addresses()
        {
            dialogService = new DialogService();
        }
        #endregion

        #region Commands
        public ICommand DeleteEmail
        {
            get
            {
                return new RelayCommand(DeleteAddressMail);
            }
        }

        async void DeleteAddressMail()
        {
            var response = await dialogService.ShowConfirm(
                "Confirm",
                Languages.ConfirmationDelete + " Email ?");
            if (!response)
            {
                return;
            }

            await JobCronExpressionViewModel.GetInstance().DeleteAddressEmail(this);
        }
        public ICommand DeleteEmailPostedReport
        {
            get
            {
                return new RelayCommand(DeleteAddressMailPostedReport);
            }
        }

        async void DeleteAddressMailPostedReport()
        {
            var response = await dialogService.ShowConfirm(
                "Confirm",
                Languages.ConfirmationDelete + " Email ?");
            if (!response)
            {
                return;
            }

            await ConfigurationPostedReportViewModel.GetInstance().DeleteAddressEmail(this);
        }
        #endregion
    }
}
