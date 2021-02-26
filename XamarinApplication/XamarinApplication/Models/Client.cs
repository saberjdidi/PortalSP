using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class Client
    {
        public int id { get; set; }
        public string companyName { get; set; }
        public string phoneNumber { get; set; }
        public string email { get; set; }
        public string code { get; set; }
        public string master { get; set; }
        public string publicId { get; set; } 
        public bool useReportCatalog { get; set; } 
        //public decimal discount { get; set; }
        //public decimal discountRequirement { get; set; }
        //public string billingRequirement { get; set; }
        //public string reference { get; set; }
        //public DateTime? startValidation { get; set; }
        //public DateTime? endValidation { get; set; }
        //public string tvaNumber { get; set; }
    }
}
