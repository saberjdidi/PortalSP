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
    public partial class InstrumentPage : ContentPage
    {
        public InstrumentPage()
        {
            InitializeComponent();
            BindingContext = new InstrumentViewModel();
        }
        private async void Add_Instrument(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new NewInstrumentPage());
        }
        private async void Update_Instrument(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Instrument instrument = ((InstrumentViewModel)BindingContext).Instruments.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            if (instrument.active == true)
            {
                await PopupNavigation.Instance.PushAsync(new UpdateInstrumentPage(instrument));
            }
            else
            {
                await PopupNavigation.Instance.PushAsync(new ShowInstrumentPage(instrument));
            }
        }
        private async void Delete_Instrument(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Instrument instrument = ((InstrumentViewModel)BindingContext).Instruments.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            if (instrument.active == true)
            {
                await PopupNavigation.Instance.PushAsync(new InstrumentDeactivatePage(instrument));
            }
            else
            {
                await PopupNavigation.Instance.PushAsync(new InstrumentActivatePage(instrument));
            }
        }
        private async void Historic_Instrument(object sender, EventArgs e)
        {
            TappedEventArgs tappedEventArgs = (TappedEventArgs)e;
            Instrument instrument = ((InstrumentViewModel)BindingContext).Instruments.Where(ser => ser.id == (int)tappedEventArgs.Parameter).FirstOrDefault();
            await PopupNavigation.Instance.PushAsync(new InstrumentHistoricPage(instrument));
        }
    }
}