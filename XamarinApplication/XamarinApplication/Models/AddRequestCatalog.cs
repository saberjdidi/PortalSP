using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class AddRequestCatalog
    {
        public string code { get; set; }
        public string description { get; set; }
        public Branch branch { get; set; }
        public Icdo icdo { get; set; }
        public Siapec siapec { get; set; }
        public Nomenclatura nomenclatura { get; set; }
        public bool valid { get; set; }
    }
}
