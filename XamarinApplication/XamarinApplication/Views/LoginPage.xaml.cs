using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.Helpers;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(Navigation);

            NavigationPage.SetHasNavigationBar(this, false);  // Hide nav bar

        }

        //close application with button Back
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();

            return false;
        }

        private async void Forgot_Password(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new ForgotPasswordPage());
        }
    }
}