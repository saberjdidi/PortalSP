using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using XamarinApplication.Services;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Models
{
    public class Requestcatalog
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public Branch branch { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public Icdo icdo { get; set; }
        public Nomenclatura nomenclatura { get; set; }
        public Siapec siapec { get; set; }
        public bool valid { get; set; }
        #endregion

        #region Constructors
        public Requestcatalog()
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
                "Are you sure to delete this Request Catalog ?");
            if (!response)
            {
                return;
            }

            await RequestCatalogViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
