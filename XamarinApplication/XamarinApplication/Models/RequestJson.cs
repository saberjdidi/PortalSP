using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class RequestJson
    {
        public long masterId { get; set; }
        public bool isCloned { get; set; }
        public List<BiologicalMaterials> biologicalMaterials { get; set; }
        public AddRequest request { get; set; }
    }
}
