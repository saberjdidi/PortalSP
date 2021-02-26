using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class AddDoctor
    {
        //public string title { get; set; }
        public string code { get; set; }
        public string fiscalCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string birthDate { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public Client client { get; set; }
        public Residence residence { get; set; }
    }
}
