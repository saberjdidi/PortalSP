using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class Residence
    {
        public long id { get; set; }
        public ComuniLocal comuniLocal { get; set; }
        public string street { get; set; }
    }
}
