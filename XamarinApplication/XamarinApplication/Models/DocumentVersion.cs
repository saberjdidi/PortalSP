using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class DocumentVersion
    {
        public long id { get; set; }
        public string docCode { get; set; }
        public string docType { get; set; }
        public string docVersion { get; set; }
        public string description { get; set; }
        public bool configured { get; set; }
    }
}
