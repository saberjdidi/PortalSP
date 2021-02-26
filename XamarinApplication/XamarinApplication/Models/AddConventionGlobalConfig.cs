using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class AddConventionGlobalConfig
    {
        public ConventionGlobalConfig conventionGlobalConfig { get; set; }
        public List<NomenclatureBilling> nomenclatureBillings { get; set; }
    }
}
