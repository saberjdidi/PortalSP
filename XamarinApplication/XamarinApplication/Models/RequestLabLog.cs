using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class RequestLabLog
    {
        public long id { get; set; }
        public DateTime? errorDate { get; set; }
        public string requestCode { get; set; } 
        public string errorNote { get; set; }
    }
}
