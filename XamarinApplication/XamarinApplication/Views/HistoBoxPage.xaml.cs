using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoBoxPage : ContentPage
    {
        public HistoBoxPage()
        {
            InitializeComponent();
            Device.OpenUri(new Uri("https://histobox.smart-path.it/index.php?page=login"));
        }
        private void HistoBox_Tapped(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://histobox.smart-path.it/index.php?page=login"));
        }
    }
}