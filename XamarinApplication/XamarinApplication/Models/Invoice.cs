using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class Invoice
    {
        public int id { get; set; }
        public string code { get; set; }
        public string patient { get; set; }
        public string contractDate { get; set; }
        public Client client { get; set; }
        public InvoiceType invoiceType { get; set; }
        public Payment payment { get; set; }
        public TVA tvaCode { get; set; }
        public double ttHt { get; set; }
        public double ttTtc { get; set; }
        public double toPayAmount { get; set; }
    }
}
