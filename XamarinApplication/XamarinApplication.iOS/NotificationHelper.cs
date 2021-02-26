using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using XamarinApplication.iOS;

[assembly: Dependency(typeof(NotificationHelper))]
namespace XamarinApplication.iOS
{
    class NotificationHelper : INotification
    {
        public void CreateNotification(string title, string message)
        {
            new NotificationDelegate().RegisterNotification(title, message);
        }
    }
}