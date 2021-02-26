using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamarinApplication.Models
{
   public class MenuItems : BindableObject
    {
        bool _isDetailVisible;

        public string Name { get; set; }
        public string Icon { get; set; }
        public Color Color { get; set; }

        public bool IsDetailVisible
        {
            get { return _isDetailVisible; }
            set
            {
                Task.Run(async () =>
                {
                    await Task.Delay(value ? 0 : 250);
                    _isDetailVisible = value;
                    OnPropertyChanged();
                });
            }
        }

        public List<MasterMenu> Items { get; set; }
    }
}
