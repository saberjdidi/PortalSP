using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class UserWrapper
    {
        public int id { get; set; }
        public string userName { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime? birthDate { get; set; }
        public string fiscalCode { get; set; }
        public bool templateConfigured { get; set; }
        public bool useReportCatalog { get; set; }
        public bool visitorPatient { get; set; }
    }
}
