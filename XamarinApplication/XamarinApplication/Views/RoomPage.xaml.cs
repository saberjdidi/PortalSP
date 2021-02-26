using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Services;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RoomPage : ContentPage
    {
        public RoomPage()
        {
            InitializeComponent();
            BindingContext = new RoomViewModel();
        }
        private async void Add_Room(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewRoomPage());
        }
        private async void Update_Room(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Room room = ((RoomViewModel)BindingContext).Rooms.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            if (room.active == true)
            {
                await PopupNavigation.Instance.PushAsync(new UpdateRoomPage(room));
            }
            else
            {
                //await Application.Current.MainPage.DisplayAlert("Room", "Room", "ok");
                await PopupNavigation.Instance.PushAsync(new ShowRoomPage(room));
            }
        }
        private async void Delete_Room(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Room room = ((RoomViewModel)BindingContext).Rooms.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            if (room.active == true)
            {
             await PopupNavigation.Instance.PushAsync(new RoomDeactivatePage(room));
            } else
            {
                await PopupNavigation.Instance.PushAsync(new RoomActivatePage(room));
            }
        }
        private async void Historic_Room(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Room room = ((RoomViewModel)BindingContext).Rooms.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
             await PopupNavigation.Instance.PushAsync(new RoomHistoricPage(room));
        }
    }
}