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
    public partial class BranchPage : ContentPage
    {
        public BranchPage()
        {
            InitializeComponent();
            BindingContext = new BranchViewModel();

        }

        private async void New_Branch(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewBranchPage());
        }

        private void Open_Menu(object sender, EventArgs e)
        {
            //IsPresented = true;
        }
    }
}