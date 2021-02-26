using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class AddTemplate
    {
        public string description { get; set; }
        public string name { get; set; }
        public string template { get; set; }
        public User createBy { get; set; }
    }
}
