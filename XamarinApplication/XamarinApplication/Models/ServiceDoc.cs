using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class ServiceDoc
    {
        public long id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public string url { get; set; } 
        public bool isActive { get; set; }
    }
}
