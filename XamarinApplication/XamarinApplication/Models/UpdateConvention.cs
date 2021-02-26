using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class UpdateConvention
    {
        public int id { get; set; }
        public TVA tva { get; set; }
        public string socialReason { get; set; }
        public string startValidation { get; set; }
        public string endValidation { get; set; }
        public string deactivationDate { get; set; }
        public int collaboratorNumber { get; set; }
        public int discount { get; set; }
        public string status { get; set; }
    }
}
