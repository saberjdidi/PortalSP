using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class RequestLog
    {
        public int id { get; set; }
        public DateTime? date { get; set; }
        public string fileName { get; set; }
        public string globalMessage { get; set; }
        public User createBy { get; set; }
    }
}
