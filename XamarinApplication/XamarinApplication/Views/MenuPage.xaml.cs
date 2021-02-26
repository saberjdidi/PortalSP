using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        public MenuPage()
        {
            InitializeComponent();
            this.Title = "GYM Application";
            BindingContext = new MasterDetailViewModel(Navigation);
            MessagingCenter.Subscribe<MasterMenu>(this, "OpenMenu", (Menu) =>
            {
               // this.Detail = new NavigationPage((Page)Activator.CreateInstance(Menu.TargetType));
               // IsPresented = false;
            });
            //time now
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                TimeNow.Text = DateTime.Now.ToString("HH.mm:ss")
                );
                return true;
            });
        }


        private void ImageButton_Clicked(object sender, EventArgs e)
        {

           // IsPresented = true;
        }

        public async void Sign_Out(object sender, EventArgs e)
        {

            var confirm = await DisplayAlert("Exit", Languages.Exit, Languages.Yes, Languages.No);
            if (confirm)
            {
                Settings.AccessToken = string.Empty;
                Debug.WriteLine(Settings.Username);
                Settings.Username = string.Empty;
                Debug.WriteLine(Settings.Password);
                Settings.Password = string.Empty;


                var client = new HttpClient();
                var response = await client.GetAsync("https://portalesp.smart-path.it/Portalesp/login/logOut");

                var token = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("-------------------token-----------------");
                Debug.WriteLine(token);
                if (token.Contains("logOut"))
                {
                    await Navigation.PushModalAsync(new LoginPage());
                }

            }
            else
            {
                await Navigation.PushModalAsync(new MainPage());
            }
        }
        private void Popup_Language(object o, EventArgs e)
        {
            PopupNavigation.Instance.PushAsync(new LanguagePopupPage());
        }
    }
}