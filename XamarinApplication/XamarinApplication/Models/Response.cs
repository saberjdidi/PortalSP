using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class Response
    {
        public string Message { get; set; }
        public int Status { get; set; }
        public bool IsSuccess { get; set; }
        public object Result { get; set; }
    }
}
