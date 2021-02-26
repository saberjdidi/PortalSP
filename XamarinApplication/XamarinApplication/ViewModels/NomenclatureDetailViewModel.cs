using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Models;

namespace XamarinApplication.ViewModels
{
    public class NomenclatureDetailViewModel
    {
        public INavigation Navigation { get; set; }
        public NomenclatureDetailViewModel(INavigation _navigation)
        {
            Navigation = _navigation;
        }
        public Nomenclatura Nomenclatura { get; set; }
    }
}
