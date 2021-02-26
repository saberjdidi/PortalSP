using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class Attachment
    {
        public string code { get; set; }
        public bool isGroup { get; set; }
        public Branch branch { get; set; }
        public Patient patient { get; set; }
        public List<Request> requests { get; set; }
    }
}
