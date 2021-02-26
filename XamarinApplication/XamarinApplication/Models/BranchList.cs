using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class BranchList
    {
        public long id { get; set; }
        public Branch branch { get; set; }
        public string extension { get; set; }
        public byte[] iconBase64 { get; set; }
    }
}
