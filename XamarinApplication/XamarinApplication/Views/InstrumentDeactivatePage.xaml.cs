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
    public partial class InstrumentDeactivatePage : PopupPage
    {
        public InstrumentDeactivatePage(Instrument instrument)
        {
            InitializeComponent();
            var viewModel = new InstrumentDeactivateViewModel();
            viewModel.Instrument = instrument;
            BindingContext = viewModel;
        }
    }
}