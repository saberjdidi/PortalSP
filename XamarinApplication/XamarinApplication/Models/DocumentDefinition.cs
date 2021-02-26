using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class DocumentDefinition
    {
        public long id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public bool isConfigured { get; set; }
        public bool isMapped { get; set; } 
        public DocumentType documentType { get; set; }
        public DocumentVersion documentVersion { get; set; }
        public ServiceDoc serviceDoc { get; set; }
    }
}
