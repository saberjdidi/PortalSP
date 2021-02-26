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
    public partial class RequestDetailPage : PopupPage
    {
        public RequestDetailPage(Request request)
        {
            InitializeComponent();

            var requestViewModel = new RequestDetailViewModel(Navigation);
            requestViewModel.RequestId = request;
            BindingContext = requestViewModel;

            var requestid = request.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = requestid }, "RequestId");
        }

        private async void Close_Popup_Request(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
}