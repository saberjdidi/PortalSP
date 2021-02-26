using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class FiscalData
    {
        public long id { get; set; }
        public string codeDestinaterio { get; set; }
        public string inderizioPec { get; set; }
        public string adresseFacturation { get; set; }
        public string conditionPaymentDescription { get; set; }
        public string conditionPayment { get; set; }
        public string pIVA { get; set; } 
    }
}
