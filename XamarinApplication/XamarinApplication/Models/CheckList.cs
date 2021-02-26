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
   public class CheckList
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string chlsCodi { get; set; }
        public string chlsDesc { get; set; }
        public bool chlsAtti { get; set; }
        public bool chlsObbl { get; set; }
        public bool chlsVali { get; set; }
        public Branch chlsTipoCodi { get; set; }
        public Icdo chlsTopoCodi { get; set; }
        public string chlsServCodi { get; set; }
        public string chlsMnem { get; set; }
        public string chlsVisi { get; set; }
        #endregion

        #region Constructors
        public CheckList()
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
                Languages.ConfirmationDelete + "CheckList" + " ?");
            if (!response)
            {
                return;
            }

            await CheckListViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
