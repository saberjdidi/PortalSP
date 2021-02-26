using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using XamarinApplication.Services;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Models
{
    public class RequestPatient
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
         public string code { get; set; }
         public bool isGroup { get; set; }
         public Branch branch { get; set; }
         public Patient patient { get; set; }
         public List<Request> requests { get; set; }
        #endregion

        #region Constructors
        public RequestPatient()
        {
            dialogService = new DialogService();
        }
        #endregion

        #region Commands
        public ICommand DownloadPreliminaryReport
        {
            get
            {
                return new RelayCommand(DownloadPdf);
            }
        }

        async void DownloadPdf()
        {

           // await RequestPatientViewModel.GetInstance().Download(this);
           // Debug.WriteLine(this.requests.Select(r => r.id).FirstOrDefault());
        }
        #endregion
    }
}
