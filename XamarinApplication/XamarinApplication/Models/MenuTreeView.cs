using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using XamarinApplication.ViewModels;

namespace XamarinApplication.Models
{
   public class MenuTreeView : BaseViewModel
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
        private ObservableCollection<MasterMenu> subFiles;
        public ObservableCollection<MasterMenu> SubFiles
        {
            get { return subFiles; }
            set
            {
                subFiles = value;
                OnPropertyChanged();
            }
        }
    }
}
