using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class Instrument
    {
        public int id { get; set; }
        public string code { get; set; }
        public bool active { get; set; }
        public User createBy { get; set; }
        public User deleteBy { get; set; }
        public DateTime createDate { get; set; }
        public DateTime deleteDate { get; set; }
        public string deleteReason { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public Status type { get; set; }
    }
}
