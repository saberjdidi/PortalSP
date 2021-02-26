using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class AttachmentHistoric
    {
        public int id { get; set; }
        public Request request { get; set; }
        public User sentBy { get; set; }
        public User returnedBy { get; set; }
        public string sendDate { get; set; }
        public string returnDate { get; set; }
    }
}
