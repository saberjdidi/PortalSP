using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class SearchRequestCatalog
    {
        public int id1 { get; set; }
        public int id2 { get; set; }
        public string order { get; set; }
        public string sortedBy { get; set; }
        public bool fromReflex { get; set; }
    }
}
