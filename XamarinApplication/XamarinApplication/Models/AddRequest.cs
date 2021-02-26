using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class AddRequest
    {
        public Requestcatalog requestCatalog { get; set; }
        public Branch branch { get; set; }
        public DateTime? checkDate { get; set; }
        public Patient patient { get; set; }
        public Nomenclatura nomenclatura { get; set; }
        public Client client { get; set; }
        public Icdo icdo { get; set; }
        public Siapec siapec { get; set; }
        public string status { get; set; }
        public string price { get; set; }
        public Instrument instrument { get; set; }
        public Room room { get; set; }
        //public User operator { get; set; }

}
}
