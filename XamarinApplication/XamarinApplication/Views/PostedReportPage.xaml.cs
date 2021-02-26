using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostedReportPage : ContentPage
    {
        public PostedReportPage()
        {
            InitializeComponent();
            BindingContext = new PostedReportViewModel();
        }
        private async void Configuration_Mail(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new ConfigurationPostedReport());
        }
    }
}