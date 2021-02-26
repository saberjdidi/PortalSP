using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class NomenclatureHistoric
    {
        public long id { get; set; }
        public User activateBy { get; set; }
        public DateTime activationDate { get; set; }
        public bool active { get; set; }
        public string deleteReason { get; set; }
        public Nomenclatura nomenclature { get; set; }
    }
}
