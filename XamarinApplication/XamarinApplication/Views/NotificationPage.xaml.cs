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
    public partial class NotificationPage : ContentPage
    {
        public NotificationPage()
        {
            InitializeComponent();
            BindingContext = new NotificationViewModel();
        }
        private async void Update_Notification(object sender, EventArgs e)
        {
            var mi = ((MenuItem)sender);
            var notification = mi.CommandParameter as Notification;
            await PopupNavigation.Instance.PushAsync(new UpdateNotificationPage(notification));
        }
    }
}