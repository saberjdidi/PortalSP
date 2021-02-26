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
   public class NomenclatureRL
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public long id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string note { get; set; } 
        #endregion

        #region Constructors
        public NomenclatureRL()
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
                Languages.ConfirmationDelete + "NomenclatureRL" + " ?");
            if (!response)
            {
                return;
            }

            await NomenclatureRLViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
