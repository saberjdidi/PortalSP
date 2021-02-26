using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class InstrumentHistoric
    {
        public int id { get; set; }
        public bool active { get; set; }
        public User activateBy { get; set; }
        public User deactivatedBy { get; set; }
        public DateTime activationDate { get; set; }
        public DateTime deactivationDate { get; set; }
        public string deleteReason { get; set; }
        public Instrument instrument { get; set; }
    }
}
