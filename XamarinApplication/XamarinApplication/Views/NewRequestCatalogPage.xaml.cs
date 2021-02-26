﻿using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewRequestCatalogPage : PopupPage
    {
        public NewRequestCatalogPage()
        {
            InitializeComponent();
            BindingContext = new NewRequestCatalogViewModel();
        }
    }
}