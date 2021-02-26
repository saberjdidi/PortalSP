using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class NewInvoiceType
    {
        public string code { get; set; }
        public string description { get; set; }
        public string note { get; set; }
        public bool valid { get; set; }
    }
}
