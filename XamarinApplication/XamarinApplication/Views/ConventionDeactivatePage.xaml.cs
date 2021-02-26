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
    public partial class ConventionDeactivatePage : PopupPage
    {
        public ConventionDeactivatePage(Convention convention)
        {
            InitializeComponent();
            var viewModel = new ConventionDeactivateViewModel();
            viewModel.Convention = convention;
            BindingContext = viewModel;
        }
    }
}