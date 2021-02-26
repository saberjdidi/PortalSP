using Rg.Plugins.Popup.Pages;
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
    public partial class LanguagePopupPage : PopupPage
    {
        public LanguagePopupPage()
        {
            BindingContext = new LanguageViewModel();
            InitializeComponent();
        }

        private async void Close_Popup_Language(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new MainPage());
            await PopupNavigation.Instance.PopAsync(true);
        }

    }
}