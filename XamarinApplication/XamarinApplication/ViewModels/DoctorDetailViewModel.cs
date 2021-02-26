using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Models;

namespace XamarinApplication.ViewModels
{
    class DoctorDetailViewModel
    {
        public INavigation Navigation { get; set; }
        public DoctorDetailViewModel(INavigation _navigation)
        {
            Navigation = _navigation;
        }
        public Doctor Doctor { get; set; }
    }
}
