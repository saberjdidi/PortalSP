using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class Branch
    {
        public long id { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        public bool isLaboratory { get; set; }
        public bool useBiologicalMaterial { get; set; }
        public bool relatedToEvents { get; set; }
    }
}
