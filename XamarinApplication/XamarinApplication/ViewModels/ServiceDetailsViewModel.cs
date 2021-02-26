using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Models;

namespace XamarinApplication.ViewModels
{
    public class ServiceDetailsViewModel
    {
        public INavigation Navigation { get; set; }
        public ServiceDetailsViewModel(INavigation _navigation)
        {
            Navigation = _navigation;
        }
        public Ambulatory Ambulatory { get; set; }
    }
}
