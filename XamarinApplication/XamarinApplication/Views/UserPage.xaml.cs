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
    public partial class UserPage : ContentPage
    {
        public UserPage()
        {
            InitializeComponent();
            BindingContext = new UserViewModel();
        }
        private async void Add_User(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewUserPage());
        }
        private async void Doctor_Detail(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var user = mi.CommandParameter as User;
            // await PopupNavigation.Instance.PushAsync(new DoctorDetailPage(user));
        }
        private async void Update_User(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var user = mi.CommandParameter as User;
            await PopupNavigation.Instance.PushAsync(new UpdateUserPage(user));
        }
        private async void Reset_User(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var user = mi.CommandParameter as User;
            await PopupNavigation.Instance.PushAsync(new ResetPasswordPage(user));
        }
    }
}