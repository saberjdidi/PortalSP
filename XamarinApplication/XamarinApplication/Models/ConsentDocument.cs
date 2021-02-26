using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using XamarinApplication.Services;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Models
{
    public class ConsentDocument
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public long id { get; set; }
        public string code { get; set; }
        public DocumentDefinition repositoryTemplate { get; set; }
        #endregion

        #region Constructors
        public ConsentDocument()
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

            await ConsentDocumentPatientViewModel.GetInstance().Download(this);
            // Debug.WriteLine(this.id);
        }
        #endregion
    }
}
