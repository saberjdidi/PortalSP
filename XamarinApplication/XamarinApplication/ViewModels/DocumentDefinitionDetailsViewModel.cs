using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Models;

namespace XamarinApplication.ViewModels
{
    public class DocumentDefinitionDetailsViewModel
    {
        public INavigation Navigation { get; set; }
        public DocumentDefinitionDetailsViewModel(INavigation _navigation)
        {
            Navigation = _navigation;
        }
        public DocumentDefinition Document { get; set; }
    }
}
