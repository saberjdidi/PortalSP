using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class SearchExecutionReport
    {
        #region Property
        public long id3 { get; set; }
        public long nomenclatureId { get; set; }
        public string criteria1 { get; set; }
        public string criteria3 { get; set; }
        public string status { get; set; }
        public string date { get; set; }
        public string date1 { get; set; }
        public int maxResult { get; set; }
        public string order { get; set; }
        public string sortedBy { get; set; }
        #endregion
    }
}
