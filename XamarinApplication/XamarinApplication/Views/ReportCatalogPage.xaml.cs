﻿using Rg.Plugins.Popup.Services;
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
    public partial class ReportCatalogPage : ContentPage
    {
        public ReportCatalogPage()
        {
            InitializeComponent();
            BindingContext = new ReportCatalogViewModel();
           // NavigationPage.SetHasNavigationBar(this, false);  // Hide nav bar
        }
        private async void Add_ReportCatalog(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewReportCatalogPage());
        }
        private async void Update_ReportCatalog(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            ReportCatalog report = ((ReportCatalogViewModel)BindingContext).ReportCatalogs.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new UpdateReportCatalogPage(report));
        }
        private async Task OpenAnimation(View view, uint length = 250)
        {
            view.RotationX = -90;
            view.IsVisible = true;
            view.Opacity = 0;
            _ = view.FadeTo(1, length);
            await view.RotateXTo(0, length);
        }

        private async Task CloseAnimation(View view, uint length = 250)
        {
            _ = view.FadeTo(0, length);
            await view.RotateXTo(-90, length);
            view.IsVisible = false;
        }
        private async void MainExpander_Tapped(object sender, EventArgs e)
        {
            var expander = sender as Expander;
            // var imgView = expander.FindByName<Grid>("ImageView");
            var detailsView = expander.FindByName<Grid>("DetailsView");

            if (expander.IsExpanded)
            {
                // await OpenAnimation(imgView);
                await OpenAnimation(detailsView);
            }
            else
            {
                await CloseAnimation(detailsView);
                // await CloseAnimation(imgView);
            }
        }
    }
}