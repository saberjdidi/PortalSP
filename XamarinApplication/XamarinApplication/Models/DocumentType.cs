using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class DocumentType
    {
        public long id { get; set; }
        public string code { get; set; }
        public string description { get; set; }
        public long docId { get; set; }
        public bool valid { get; set; }
    }
}
