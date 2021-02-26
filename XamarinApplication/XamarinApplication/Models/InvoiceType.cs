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
   public class InvoiceType
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public long id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string note { get; set; }
        public bool valid { get; set; }
        public bool isDefault { get; set; }
        #endregion

        #region Constructors
        public InvoiceType()
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
                Languages.Confirm,
                Languages.ConfirmationDelete + "Invoice Type" + " ?");
            if (!response)
            {
                return;
            }

            await InvoiceTypeViewModel.GetInstance().Delete(this);
        }
        public ICommand CheckedCommand
        {
            get
            {
                return new RelayCommand(Checked);
            }
        }
        async void Checked()
        {
            await InvoiceTypeViewModel.GetInstance().ChangeChecked(this);
        }
        #endregion
    }
}
