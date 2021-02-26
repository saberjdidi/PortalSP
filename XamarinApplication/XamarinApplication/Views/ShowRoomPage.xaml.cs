using Rg.Plugins.Popup.Pages;
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
    public partial class ShowRoomPage : PopupPage
    {
        public ShowRoomPage(Room room)
        {
            InitializeComponent();
            var viewModel = new UpdateRoomViewModel();
            viewModel.Room = room;
            BindingContext = viewModel;
        }
    }
}