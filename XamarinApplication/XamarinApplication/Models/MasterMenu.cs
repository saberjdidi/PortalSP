using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace XamarinApplication.Models
{
    public class MasterMenu : INotifyPropertyChanged
    {
        string _menuIcon;
        public string MenuIcon
        {
            get
            {
                return _menuIcon;
            }
            set
            {
                if (value != null)
                {
                    _menuIcon = value;
                    OnPropertyChanged();
                }
            }
        }
        public string MenuName { get; set; }
        bool _selected;
        public bool Selected
        {
            get
            {
                return _selected;
            }
            set
            {
                _selected = value;
                OnPropertyChanged();
            }
        }
        public Type TargetType { get; set; }

        public Color Color { get; set; }
        public bool Done { get; set; }
        public bool IsLatest { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
