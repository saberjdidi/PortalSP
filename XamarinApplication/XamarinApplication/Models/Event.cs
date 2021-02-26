using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class Event
    {
        public long id { get; set; }
        public string code { get; set; }
        public DateTime? dateEvent { get; set; }
        public Patient patient { get; set; }
        public TypeEvent typeEvent { get; set; }
    }
}
