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
    public partial class UpdateICDOPage : PopupPage
    {
        public UpdateICDOPage(Icdo icdo)
        {
            InitializeComponent();
            var updateIcdoViewModel = new UpdateICDOViewModel();
            updateIcdoViewModel.Icdo = icdo;
            BindingContext = updateIcdoViewModel;
        }
    }
}