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
    public class Doctor
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string title { get; set; }
        public string code { get; set; }
        public string fiscalCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string birthDate { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public Client client { get; set; }
        public Residence residence { get; set; }
        #endregion

        #region Constructors
        public Doctor()
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
                Languages.ConfirmationDelete + Languages.Doctor +" ?");
            if (!response)
            {
                return;
            }

            await DoctorViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
