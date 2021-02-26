using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
   public class AddJobCron
    {
        public List<AddConfigs> configs { get; set; }
        public List<AddAddress> addresses { get; set; }
    }
    public class AddConfigs
    {
        public string code { get; set; }
        public string cron { get; set; }
    }
    public class AddAddress
    {
        public string code { get; set; }
        public string addressMail { get; set; }
        public bool duplicate { get; set; }
    }
}
