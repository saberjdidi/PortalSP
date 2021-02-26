using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.Helpers;
using XamarinApplication.ViewModels;
using XamarinApplication.Views;

namespace XamarinApplication
{
    public partial class App : Application
    {
        public static string CurrentLanguage { get; internal set; }

        public App()
        {
            //Register Syncfusion license
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTk2OTUzQDMxMzcyZTM0MmUzMEFsbVhhZG54VUZ3K252TlZodTRZeXBqMWJibEZvM3hkSTdwZVhSQ2p1T289");
            InitializeComponent();
            Device.SetFlags(new[] { "Expander_Experimental" });
            //Device.SetFlags(new[] { "SwipeView_Experimental" });
            MainPage = new NavigationPage(new LoginPage());
            //SetMainPage();
        }

        private void SetMainPage()
        {
            if(!string.IsNullOrEmpty(Settings.AccessToken))
            {
                if (Settings.AccessTokenExpirationDate < DateTime.UtcNow.AddHours(1))
                {
                    var loginViewModel = new LoginViewModel();
                    loginViewModel.LoginCommand.Execute(null);
                }
                MainPage = new MainPage();
            }
            /*else if(!string.IsNullOrEmpty(Settings.Username) 
                    && !string.IsNullOrEmpty(Settings.Password))
            {
                MainPage = new NavigationPage(new LoginPage());
            }*/
            else
            {
                MainPage = new NavigationPage(new LoginPage()); //RegisterPage
            }
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
