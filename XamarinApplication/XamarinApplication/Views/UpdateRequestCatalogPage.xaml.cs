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
    public partial class UpdateRequestCatalogPage : PopupPage
    {
        public UpdateRequestCatalogPage(Requestcatalog requestcatalog)
        {
            InitializeComponent();
            var viewModel = new UpdateRequestCatalogViewModel();
            viewModel.RequestcatalogId = requestcatalog;
            BindingContext = viewModel;

            var requestcatalogid = requestcatalog.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = requestcatalogid }, "UpdateRequestCatalogId");
        }
    }
}