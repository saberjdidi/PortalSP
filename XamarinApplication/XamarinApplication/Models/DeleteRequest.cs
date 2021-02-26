using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class DeleteRequest
    {
        public long id { get; set; }
        public string reason { get; set; }
    }
}
