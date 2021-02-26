using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class AddSiapec
    {
        public string code { get; set; }
        public string description { get; set; }
        public Branch branch { get; set; }
        public CodRL codRL { get; set; }
        //public bool isExist { get; set; }
    }
}
