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
    public partial class UpdateUserPage : PopupPage
    {
        public UpdateUserPage(User user)
        {
            InitializeComponent();
            var viewModel = new UpdateUserViewModel();
            viewModel.UserId = user;
            BindingContext = viewModel;

            var userid = user.id;
            MessagingCenter.Send(new PassIdPatient() { idPatient = userid }, "UpdateUserId");
        }
    }
}