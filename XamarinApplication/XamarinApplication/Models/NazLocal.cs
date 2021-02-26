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
   public class NazLocal
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string codVal { get; set; }
        public bool cee { get; set; }
        public int lunVal1 { get; set; }
        public int lunVal2 { get; set; }
        #endregion

        #region Constructors
        public NazLocal()
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
                Languages.ConfirmationDelete + " Naz Local ?");
            if (!response)
            {
                return;
            }

            await NazLocalViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
