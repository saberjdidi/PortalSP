using Plugin.Multilingual;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using XamarinApplication.Models;
using XamarinApplication.Resources;

namespace XamarinApplication.ViewModels
{
    public class LanguageViewModel : INotifyPropertyChanged
    {
        public LanguageViewModel()
        {
            CitiesList = GetLanguage().OrderBy(t => t.Value).ToList();
        }
        public List<Language> CitiesList { get; set; }
        private Language _selectedCity { get; set; }
        public Language SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                if (_selectedCity != value)
                {
                    _selectedCity = value;
                    // Do whatever functionality you want...When a selectedItem is changed..
                    // write code here..
                    Resource.Culture = new CultureInfo(_selectedCity.Key);
                    CrossMultilingual.Current.CurrentCultureInfo = new CultureInfo(_selectedCity.Key);
                }
            }
        }
        public List<Language> GetLanguage()
        {
            var languages = new List<Language>()
            {
                new Language(){Key =  "en", Value= "English"},
                new Language(){Key =  "fr", Value= "Francais"},
                new Language(){Key =  "it", Value= "Italian"}
            };

            return languages;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
