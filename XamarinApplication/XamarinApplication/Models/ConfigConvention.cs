using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class ConfigConvention
    {
        public string client { get; set; }
        public List<ConfigsConvention> configs { get; set; }
        public bool hide { get; set; }
        public bool isGroup { get; set; }
    }
}
