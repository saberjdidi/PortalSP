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
   public class AmbulatoryRequest
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public Ambulatory ambulatory { get; set; }
        public string sendDate { get; set; }
        public DateTime returnDate { get; set; }
        public User sentBy { get; set; }
        public User returnedBy { get; set; }
        #endregion

        #region Constructors
        public AmbulatoryRequest()
        {
            dialogService = new DialogService();
        }
        #endregion

    }
}
