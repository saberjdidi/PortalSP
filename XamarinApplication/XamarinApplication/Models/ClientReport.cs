using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class ClientReport
    {
        public int id { get; set; }
        public string code { get; set; }
        public string reportName { get; set; }
        public DateTime creationDate { get; set; }
        public bool saved { get; set; }
        public bool reportIsConfigured { get; set; }
    }
}
