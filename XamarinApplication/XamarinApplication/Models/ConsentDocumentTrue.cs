using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using XamarinApplication.Services;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Models
{
   public class ConsentDocumentTrue
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public long id { get; set; }
        public ConsentDocument document { get; set; }
        public DateTime? saveDate { get; set; }
        public Patient patient { get; set; }
        #endregion

        #region Constructors
        public ConsentDocumentTrue()
        {
            dialogService = new DialogService();
        }
        #endregion

        #region Commands
        public ICommand DownloadConsentDocument
        {
            get
            {
                return new RelayCommand(DownloadWord);
            }
        }

        async void DownloadWord()
        {

            await ConsentDocumentPatientTrueViewModel.GetInstance().Download(this);
            // Debug.WriteLine(this.id);
        }
        #endregion
    }
}
