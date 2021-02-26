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
    public class User
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string userName { get; set; }
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public bool enabled { get; set; }
        public Client client { get; set; }
        public Role role { get; set; }
        public string creationDate { get; set; }
        public string fiscalCode { get; set; }

        /*public long defaultClientId { get; set; }
        public DateTime? birthDate { get; set; }
        public string companyName { get; set; }
        public string stateOfBirth { get; set; }
        public string provinceOfBirth { get; set; }
        public string placeOfBirth { get; set; }
        public string cellPhone { get; set; }
        public string companyRole { get; set; }
        public string companyAddress { get; set; }
        public string companyCity { get; set; }*/
        #endregion

        #region Constructors
        public User()
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
                Languages.ConfirmationDelete+" "+ Languages.User + " ?");
            if (!response)
            {
                return;
            }

            await UserViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
