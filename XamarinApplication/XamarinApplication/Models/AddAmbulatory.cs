using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class AddAmbulatory
    {
        public string code { get; set; }
        public string description { get; set; }
        public string domicile { get; set; }
        public string residence { get; set; }
        public string zipCode { get; set; }
        public string phone { get; set; }
        public string tvaCode { get; set; }
        public DateTime birthDate { get; set; }
    }
}
