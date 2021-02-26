using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewJobCronDaysPage : PopupPage
    {
        List<JobCronDays> list = new List<JobCronDays>();
        public NewJobCronDaysPage()
        {
            InitializeComponent();

            list.Add(new JobCronDays() { Text = "MON" });
            list.Add(new JobCronDays() { Text = "TUE" });
            list.Add(new JobCronDays() { Text = "WED" });
            list.Add(new JobCronDays() { Text = "THU" });
            list.Add(new JobCronDays() { Text = "FRI" });
            list.Add(new JobCronDays() { Text = "SAT" });
            list.Add(new JobCronDays() { Text = "SUN" });

            listView.ItemsSource = list;
        }
        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();

            var result = list.Where(w => w.IsChecked == true).ToList();

            string s = "";

            int index = 0;
            foreach (var model in result)
            {
                s = s + model.Text;
                if (index < result.Count - 1)
                {
                    s = s + ",";
                }
                index++;
            }

            MessagingCenter.Send<object, string>(this, "Hi", s);
        }

        private async void Close_Popup(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PopAsync(true);
        }
    }
    public class JobCronDays : BaseViewModel
    {
        public JobCronDays()
        {
            IsChecked = false;
        }

        public string Text { get; set; }

        private bool isChecked;
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
                OnPropertyChanged();
            }
        }
    }
}