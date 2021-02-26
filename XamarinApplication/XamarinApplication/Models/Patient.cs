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
    public class Patient
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string title { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string birthDate { get; set; }
        public string fiscalCode { get; set; }
        public string phone { get; set; }
        public string cellPhone { get; set; }
        public string email { get; set; }
        //public string mpiCode { get; set; }
        //public DateTime? deleteDate { get; set; }
        public Gender gender { get; set; }
        public ComuniLocal placeOfBirth { get; set; }
        public Client client { get; set; }
        public FiscalData fiscalData { get; set; }
        public Domicile domicile { get; set; }
        public Residence residence { get; set; }
        public string note { get; set; }
        public bool confirmSave { get; set; }
        public bool isMerged { get; set; }
        public bool isRepositorySaved { get; set; } 
        #endregion

        #region Constructors
        public Patient()
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
                Languages.ConfirmationDelete+" "+ Languages.Patient + " ?");
            if (!response)
            {
                return;
            }

            await PatientViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
