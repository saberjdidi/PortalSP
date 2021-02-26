using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.ViewModels;
using XamarinApplication.Views;

namespace XamarinApplication
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]


    public partial class MainPage : MasterDetailPage
    {
        public User User { get; set; }
        //private const string URL = "http://phoneofficine.it/niini-gim/planning";
        //private HttpClient _client = new HttpClient();
        //private ObservableCollection<Planning> _plannings;
        private Timer timer;
        
        public MainPage()
        {
            InitializeComponent();
            this.Title = "GYM Application";
            BindingContext = new MasterDetailViewModel(Navigation);
            MessagingCenter.Subscribe<MasterMenu>(this, "OpenMenu", (Menu) =>
            {
                this.Detail = new NavigationPage((Page)Activator.CreateInstance(Menu.TargetType));
                IsPresented = false;
            });

            //Name of XAML (label, Button, etc.)
            //Logout.Text = Languages.Logout;

            //time now
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                Device.BeginInvokeOnMainThread(() =>
                TimeNow.Text = DateTime.Now.ToString("HH.mm:ss")
                );
                return true;
            });

            //show hide dweb image
            var Username = Settings.Username;
            User = JsonConvert.DeserializeObject<User>(Username);
            if (User.role.authority.Equals("ADMIN_ROLE") || User.role.authority.Equals("DOCTORCLIENT_ROLE"))
            {
                dwebCatalog.IsVisible = true;
            }
            else
            {
                dwebCatalog.IsVisible = false;
            }
        }

        protected override void OnAppearing()
        {
            (this.BindingContext as MasterDetailViewModel).GetCarousels();
            timer = new Timer(TimeSpan.FromSeconds(5).TotalMilliseconds) { AutoReset = true, Enabled = true };
            timer.Elapsed += Timer_Elapsed;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            timer?.Dispose();
            base.OnDisappearing();
        }
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {

                if (cvCarousels.Position == 4)
                {
                    cvCarousels.Position = 0;
                    return;
                }

                cvCarousels.Position += 1;
            });
        }

        
        private void ImageButton_Clicked(object sender, EventArgs e)
        {
          
            IsPresented = true;
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

        protected void DWEB_Catalog(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://fatture.datalog.it/#"));
        }
    }

}
