using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class DoctorNoRef
    {
        public int id { get; set; }
        public string fiscalCode { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string birthDate { get; set; }
        public string email { get; set; }
        public string cellPhone { get; set; }
        public string creationDate { get; set; }
        public Client client { get; set; }
        public List<Client> clientsToManage { get; set; }
    }
}
