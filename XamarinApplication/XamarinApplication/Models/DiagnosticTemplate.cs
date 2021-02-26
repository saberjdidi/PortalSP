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
   public class DiagnosticTemplate
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string template { get; set; }
        public User createBy { get; set; }
        #endregion

        #region Constructors
        public DiagnosticTemplate()
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
                Languages.ConfirmationDelete + "Diagnostic Template" + " ?");
            if (!response)
            {
                return;
            }

            await DiagnosticTemplateViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
