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
    public partial class InstrumentActivatePage : PopupPage
    {
        public InstrumentActivatePage(Instrument instrument)
        {
            InitializeComponent();
            var viewModel = new InstrumentActivateViewModel();
            viewModel.Instrument = instrument;
            BindingContext = viewModel;
        }
    }
}