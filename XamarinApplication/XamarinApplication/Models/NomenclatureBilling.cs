using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class NomenclatureBilling
    {
        public long id { get; set; }
        public Nomenclatura nomenclatura { get; set; }
        public string price { get; set; }
    }
}
