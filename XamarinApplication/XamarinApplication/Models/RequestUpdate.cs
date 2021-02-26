using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class RequestUpdate
    {
        public List<BiologicalMaterials> biologicalMaterials { get; set; }
        public Request request { get; set; }
    }
}
