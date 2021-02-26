using System;
using Rg.Plugins.Popup.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.ViewModels;
using XamarinApplication.Models;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ServiceDocumentPage : ContentPage
    {
        public ServiceDocumentPage()
        {
            InitializeComponent();
            BindingContext = new ServiceDocumentViewModel();
        }
        private async void Add_ServiceDocument(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewServiceDocumentPage());
        }
        private async void Update_ServiceDocument(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            ServiceDocument serviceDocument = ((ServiceDocumentViewModel)BindingContext).ServiceDocuments.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new UpdateServiceDocumentPage(serviceDocument));
        }
    }
}