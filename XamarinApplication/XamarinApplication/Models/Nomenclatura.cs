using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class Nomenclatura
    {
        public long id { get; set; }
        public string code { get; set; }
        public string descrEsameFunz { get; set; }
        public string descrEsameProf { get; set; }
        public string executionRules { get; set; }
        public DateTime startValidation { get; set; }
        public DateTime endValidation { get; set; }
        public Siapec siapec { get; set; }
        public string ptsyn { get; set; }
        //public double totalCost { get; set; }
        //public double totalCostProposed { get; set; }
        public Branch branch { get; set; }
        public Icdo icdo { get; set; }
    }
}
