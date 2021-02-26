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
    public partial class AntecedentPage : ContentPage
    {
        public AntecedentPage()
        {
            InitializeComponent();
            BindingContext = new AntecedentViewModel();
        }
        private async void Add_Antecedent(object sender, EventArgs e)
        {
           await PopupNavigation.Instance.PushAsync(new NewAntecedentPage());
        }
        private async void Update_Antecedent(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Antecedent antecedent = ((AntecedentViewModel)BindingContext).Antecedents.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new UpdateAntecedentPage(antecedent));
        }
    }
}