using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class AddCheckList
    {
        public string chlsCodi { get; set; }
        public string chlsDesc { get; set; }
        public bool chlsAtti { get; set; }
        public string chlsMnem { get; set; }
        public Branch chlsTipoCodi { get; set; }
        public Icdo chlsTopoCodi { get; set; }
    }
}
