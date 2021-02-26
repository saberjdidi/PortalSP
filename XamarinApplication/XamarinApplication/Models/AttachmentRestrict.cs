using GalaSoft.MvvmLight.Command;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using XamarinApplication.Helpers;
using XamarinApplication.Services;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Models
{
   public class AttachmentRestrict
    {
        #region Services
        DialogService dialogService;
        #endregion

        #region Properties
        public int id { get; set; }
        public User createBy { get; set; }
        public User annulledBy { get; set; } 
        public string creationDate { get; set; } 
        public string annulledDate { get; set; }
        public Request request { get; set; }
        public string name { get; set; }
        public string extension { get; set; }
        public bool visible { get; set; }
        public bool positive { get; set; }
        public bool downloaded { get; set; }
        public bool patientNotified { get; set; }
        public User User { get; set; }
        #endregion

        #region Constructors
        public AttachmentRestrict()
        {
            dialogService = new DialogService();
        }
        #endregion

        #region Commands
        //AttachmentRestrict
        public ICommand DownloadPDFAttachment
        {
            get
            {
                return new RelayCommand(DownloadPdf);
            }
        }

        async void DownloadPdf()
        {

             await AttachmentListRestrictViewModel.GetInstance().Download(this);
           // Debug.WriteLine(this.id);
        }

        //Attachment Request
        public ICommand DownloadPDFAttachmentRequest
        {
            get
            {
                return new RelayCommand(DownloadPdfRequest);
            }
        }

        async void DownloadPdfRequest()
        {
            var Username = Settings.Username;
            User = JsonConvert.DeserializeObject<User>(Username);
            if (User.role.authority.Equals("ADMIN_ROLE"))
            {
                await RequestAttachmentViewModel.GetInstance().Download(this);
            } else
            {
                await RequestAttachmentRoleViewModel.GetInstance().Download(this);
            }
                
            // Debug.WriteLine(this.id);
        }
        #endregion
    }
}
