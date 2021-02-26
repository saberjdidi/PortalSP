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
   public class Notification
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string code { get; set; }
        public DateTime endValidationDate { get; set; }
        public string message { get; set; }
        #endregion

        #region Constructors
        public Notification()
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
                Languages.ConfirmationDelete + " Notification ?");
            if (!response)
            {
                return;
            }

            await NotificationViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
