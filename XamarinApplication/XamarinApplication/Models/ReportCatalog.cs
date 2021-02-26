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
   public class ReportCatalog
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public Icdo icdo { get; set; }
        public RagService ragService { get; set; }
        public object siapecs { get; set; }
        #endregion

        #region Constructors
        public ReportCatalog()
        {
            dialogService = new DialogService();
            siapecs = new List<Siapec>();
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
                Languages.ConfirmationDelete + " Report ?");
            if (!response)
            {
                return;
            }

            await ReportCatalogViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
