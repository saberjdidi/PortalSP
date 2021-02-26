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
    public partial class UpdateNomenclatureRLPage : PopupPage
    {
        public UpdateNomenclatureRLPage(NomenclatureRL nomenclature)
        {
            InitializeComponent();
            var viewModel = new UpdateNomenclatureRLViewModel();
            viewModel.NomenclatureRL = nomenclature;
            BindingContext = viewModel;
        }
    }
}