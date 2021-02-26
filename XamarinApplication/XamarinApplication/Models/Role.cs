using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class Role
    {
        public int id { get; set; }
        public string authority { get; set; }
        public TypeRole type { get; set; }
    }
}
