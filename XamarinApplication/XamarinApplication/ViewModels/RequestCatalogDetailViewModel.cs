using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Models;

namespace XamarinApplication.ViewModels
{
    public class RequestCatalogDetailViewModel
    {
        public INavigation Navigation { get; set; }
        public RequestCatalogDetailViewModel(INavigation _navigation)
        {
            Navigation = _navigation;
        }
        public Requestcatalog RequestCatalog { get; set; }
    }
}
