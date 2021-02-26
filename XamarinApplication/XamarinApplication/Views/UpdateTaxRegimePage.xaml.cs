using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.Models;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdateTaxRegimePage : PopupPage
    {
        public UpdateTaxRegimePage(TaxRegime taxRegime)
        {
            InitializeComponent();
            var viewModel = new UpdateTaxRegimeViewModel();
            viewModel.TaxRegime = taxRegime;
            BindingContext = viewModel;
        }
    }
}