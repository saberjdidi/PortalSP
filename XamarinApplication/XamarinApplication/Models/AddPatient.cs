using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class AddPatient
    {
        public string title { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string fullName { get; set; }
        public string fiscalCode { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string cellPhone { get; set; }
        // public DateTime birthDate { get; set; }
        public string birthDate { get; set; }
        public ComuniLocal placeOfBirth { get; set; }
        public Client client { get; set; }
        //public FiscalData fiscalData { get; set; }
        public AddDomicile domicile { get; set; }
        public AddResidence residence { get; set; }
        public string note { get; set; }
    }
}
