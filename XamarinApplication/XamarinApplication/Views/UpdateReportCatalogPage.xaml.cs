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
    public partial class UpdateReportCatalogPage : PopupPage
    {
        public UpdateReportCatalogPage(ReportCatalog report)
        {
            InitializeComponent();
            var viewModel = new UpdateReportCatalogViewModel();
            viewModel.ReportCatalogId = report;
            BindingContext = viewModel;

            var reportid = report.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = reportid }, "UpdateReportId");
        }
    }
}