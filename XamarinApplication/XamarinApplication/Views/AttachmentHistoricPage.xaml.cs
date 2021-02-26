using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
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
    public partial class AttachmentHistoricPage : PopupPage
    {
        public AttachmentHistoricPage(Attachment attachment)
        {
            InitializeComponent();
            var viewModel = new AttachmentHistoricViewModel();
            viewModel.Attachment = attachment;
            BindingContext = viewModel;
        }
        private async void Close_Popup_Request(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}