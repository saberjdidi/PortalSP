using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class RequestLogHistoric
    {
        public int id { get; set; }
        public string requestCode { get; set; }
        public bool isExist { get; set; }
        public List<ErrorMessage> errorMessages { get; set; }
    }
}
