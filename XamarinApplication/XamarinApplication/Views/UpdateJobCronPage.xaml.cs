﻿using Rg.Plugins.Popup.Extensions;
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
    public partial class UpdateJobCronPage : PopupPage
    {
        public UpdateJobCronPage(Configs configs)
        {
            InitializeComponent();
            var viewModel = new UpdateJobCronViewModel();
            viewModel.Configs = configs;
            BindingContext = viewModel;
        }
        private async void Entry_Focused(object sender, FocusEventArgs e)
        {
            await Navigation.PushPopupAsync(new NewJobCronDaysPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            MessagingCenter.Subscribe<object, string>(this, "Hi", (obj, s) => {
                entry.Text = s;
            });
        }
    }
}