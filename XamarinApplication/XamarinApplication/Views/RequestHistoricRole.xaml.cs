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
    public partial class RequestHistoricRole : PopupPage
    {
        public RequestHistoricRole(Attachment attachment)
        {
            InitializeComponent();
            var requestViewModel = new RequestHistoricRoleViewModel();
            requestViewModel.AttachmentId = attachment;
            BindingContext = requestViewModel;

            var requestid = attachment.requests.Select(r=>r.id).FirstOrDefault();
            MessagingCenter.Send(new PassIdPatient() { idPatient = requestid }, "RequestId");
        }
        private async void Close_Popup_Request(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}