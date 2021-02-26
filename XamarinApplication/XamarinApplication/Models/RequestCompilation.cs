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
   public class RequestCompilation
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string code { get; set; }
        public string reportName { get; set; }
        public DateTime creationDate { get; set; }
        public bool reportIsConfigured { get; set; }
        public bool saved { get; set; }
        #endregion

        #region Constructors
        public RequestCompilation()
        {
            dialogService = new DialogService();
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
            var response = await dialogService.ShowConfirm(
                "Confirm",
                Languages.ConfirmationDelete + " Request Compilation ?");
            if (!response)
            {
                return;
            }

            await RequestCompilationViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
