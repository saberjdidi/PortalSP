using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class UpdateConventionGlobalConfig
    {
        public ConfigurationConvention conventionGlobalConfig { get; set; }
        public List<NomenclatureBilling> nomenclatureBillings { get; set; }

        #region Constructors
        public UpdateConventionGlobalConfig()
        {
           // nomenclatureBillings = new List<NomenclatureBilling>();
        }
        #endregion
    }
}
