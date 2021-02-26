using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace XamarinApplication.Models
{
    public class AddBranchImage
    {
        public AddBranch branch { get; set; }
        public string extension { get; set; }
        public string name { get; set; }
        public MultipartFormDataContent fileData { get; set; }
        //public byte[] fileData { get; set; }
        //public MediaFile fileData;
    }
}
