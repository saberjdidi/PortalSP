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
    public class Siapec
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public long id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public Branch branch { get; set; }
        public CodRL codRL { get; set; }
        public bool isExist { get; set; }
        #endregion

        #region Constructors
        public Siapec()
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
                Languages.ConfirmationDelete + " SIAPEC ?");
            if (!response)
            {
                return;
            }

            await SiapecViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
