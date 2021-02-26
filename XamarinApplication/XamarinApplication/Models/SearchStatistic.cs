using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class SearchStatistic
    {
        public int id1 { get; set; }
        public int id2 { get; set; }
        public int id3 { get; set; }
        public int conventionId { get; set; }
        public int nomenclatureId { get; set; }
        public string criteria0 { get; set; }
        public string criteria1 { get; set; }
        public string criteria2 { get; set; }
        public string criteria3 { get; set; }
        public string criteria4 { get; set; }
        public string status { get; set; }
        public string date { get; set; }
        public string date1 { get; set; }
        public bool masterControl { get; set; }
        public bool positive { get; set; }
        public int maxResult { get; set; }
        public int offset { get; set; }
        public string order { get; set; }
        public string sortedBy { get; set; }
    }
}
