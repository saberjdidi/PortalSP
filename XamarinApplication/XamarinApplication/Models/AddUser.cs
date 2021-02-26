using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class AddUser
    {
        public string userName { get; set; }
        public string password { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string fiscalCode { get; set; }
        public bool enabled { get; set; }
        public Client client { get; set; }
        public Role role { get; set; }
    }
}
