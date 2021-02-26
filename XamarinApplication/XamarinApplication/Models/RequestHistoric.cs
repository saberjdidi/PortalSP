using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class RequestHistoric
    {
        public long id { get; set; }
        public DateTime? actionDate { get; set; }
        public string clientCompanyName { get; set; }
        public string createByFirstName { get; set; }
        public string deleteByFirstName { get; set; }
        public string patientFirstName { get; set; }
        public string patientFullName { get; set; }
        public string patientLastName { get; set; }
        public string removedRegroupmentFirstName { get; set; }
        public string requestCode { get; set; }
        public Status status { get; set; }
    }
}
