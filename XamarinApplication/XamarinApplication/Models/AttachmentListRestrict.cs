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
   public class AttachmentListRestrict
    {

        #region Properties
        public List<AttachmentRestrict> attachments { get; set; }
        public bool canControlAttachment { get; set; }
        public bool canControlAttachmentPositivity { get; set; }
        public bool canSendEmailNotification { get; set; }
        public int patientRelationToClient { get; set; }
        #endregion

    }
}
