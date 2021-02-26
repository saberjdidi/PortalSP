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
   public class ClosureCalendar
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public string code { get; set; }
        public string period { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        #endregion

        #region Constructors
        public ClosureCalendar()
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
                Languages.ConfirmationDelete + "Closure Calendar" + " ?");
            if (!response)
            {
                return;
            }

            await ClosureCalendarViewModel.GetInstance().Delete(this);
        }
        #endregion
    }
}
