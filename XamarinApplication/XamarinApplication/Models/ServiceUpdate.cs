using System;
using System.Collections.Generic;
using System.Text;

namespace XamarinApplication.Models
{
    public class ServiceUpdate
    {
        #region Properties
        public Ambulatory ambulatory { get; set; }
        public object usresWrapper { get; set; }
        #endregion

        #region Constructors
        public ServiceUpdate()
        {
            usresWrapper = new List<UserWrapper>();
        }
        #endregion
    }
}
