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
    public partial class ConventionPage : ContentPage
    {
        public bool DeleteConvention { get; set; }
        public ConventionPage()
        {
            InitializeComponent();
            BindingContext = new ConventionViewModel();
            // NavigationPage.SetHasNavigationBar(this, false);
            
        }
        private async void configuration_global_convention(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new ConfigurationGlobaleConvention());
        }
        private async void Add_Convention(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewConventionPage());
        }
        private async void Update_Convention(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Convention convention = ((ConventionViewModel)BindingContext).Conventions.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new UpdateConventionPage(convention));
        }
        private async void Deactivate_Convention(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Convention convention = ((ConventionViewModel)BindingContext).Conventions.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            if(convention.status.name == "AC")
            {
                //gridDeactivate.IsVisible = true;
                DeleteConvention = true;
                await PopupNavigation.Instance.PushAsync(new ConventionDeactivatePage(convention));
            }
            else
            {
                //gridDeactivate.IsVisible = false;
                DeleteConvention = false;
            }
        }
        private async void Configuration_Convention(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Convention convention = ((ConventionViewModel)BindingContext).Conventions.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
             await PopupNavigation.Instance.PushAsync(new ConfigurationConventionPage(convention));
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