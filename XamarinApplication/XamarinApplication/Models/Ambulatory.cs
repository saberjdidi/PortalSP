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
    public class Ambulatory
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public string domicile { get; set; }
        public string residence { get; set; }
        public string zipCode { get; set; }
        public string phone { get; set; }
        public string tvaCode { get; set; }
        #endregion

        #region Constructors
        public Ambulatory()
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
                Languages.ConfirmationDelete + " Service ?");
            if (!response)
            {
                return;
            }

            await ServiceViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
