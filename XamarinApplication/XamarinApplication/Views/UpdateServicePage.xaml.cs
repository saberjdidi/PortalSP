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
    public partial class UpdateServicePage : PopupPage
    {
        public UpdateServicePage(Ambulatory ambulatory)
        {
            InitializeComponent();
            var viewModel = new UpdateServiceViewModel();
            viewModel.Ambulatory = ambulatory;
            BindingContext = viewModel;

            var ambulatoryid = ambulatory.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = ambulatoryid }, "UpdateServiceId");
        }
    }
}