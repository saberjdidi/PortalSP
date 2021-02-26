using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class RoomUpdate
    {
        public int id { get; set; }
        public string code { get; set; }
        public bool active { get; set; }
        public User createBy { get; set; }
        public User deleteBy { get; set; }
       // public string createDate { get; set; }
       // public string deleteDate { get; set; }
        public string deleteReason { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }
}
