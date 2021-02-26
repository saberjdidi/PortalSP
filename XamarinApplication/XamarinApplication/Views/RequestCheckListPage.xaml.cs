using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public partial class RequestCheckListPage : PopupPage
    {
        public RequestCheckListPage(Request request)
        {
            InitializeComponent();
            var viewModel = new RequestCheckListViewModel();
            viewModel.Request = request;
            BindingContext = viewModel;
        }
        private async void Close_Popup(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
        private async void CheckList_Symptoms(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            CheckList checkList = ((RequestCheckListViewModel)BindingContext).CheckList.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
             await PopupNavigation.Instance.PushAsync(new RequestCheckListSymptoms(checkList));
            //Debug.WriteLine("********checkList*************");
            //Debug.WriteLine(checkList.id);
        }
    }
}