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
   public class Convention
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public TVA tva { get; set; }
        public string socialReason { get; set; }
        public DateTime startValidation { get; set; }
        public DateTime endValidation { get; set; }
        public DateTime deactivationDate { get; set; }
        public int collaboratorNumber { get; set; }
        public int discount { get; set; }
        public Status status { get; set; }
        #endregion

        #region Constructors
        public Convention()
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
                Languages.ConfirmationDelete + " Convention ?");
            if (!response)
            {
                return;
            }

            await ConventionViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
