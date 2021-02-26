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
    public partial class AttachmentsPage : ContentPage
    {
        public AttachmentsPage()
        {
            InitializeComponent();
            BindingContext = new AttachmentsViewModel();
        }
        private async void Attachment_Detail(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            await PopupNavigation.Instance.PushAsync(new AttachmentListRestrict(attachment));
        }
        private async void Attachment_Historic(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var attachment = mi.CommandParameter as Attachment;
            await PopupNavigation.Instance.PushAsync(new AttachmentHistoricPage(attachment));
        }
        /* protected override void OnAppearing()
{
    (this.BindingContext as AttachmentsViewModel).GetAttachments();
}*/
    }
}