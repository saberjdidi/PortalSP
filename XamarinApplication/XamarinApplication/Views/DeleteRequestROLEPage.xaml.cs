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
    public partial class DeleteRequestROLEPage : PopupPage
    {
        public DeleteRequestROLEPage(Attachment attachment)
        {
            InitializeComponent();
            InitializeComponent();
            var deleteRequestViewModel = new DeleteRequestROLEViewModel();
            deleteRequestViewModel.Attachment = attachment;
            BindingContext = deleteRequestViewModel;
        }
        private async void Close_Popup(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}