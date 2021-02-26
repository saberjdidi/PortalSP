using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;

namespace XamarinApplication.Converters
{
   public class TemplateUserVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && value != null)
            {
                string s = (string)value;
                switch (s)
                {
                    case "DOCTOR":
                        return true;
                    case "ADMIN":
                        return false;
                    default:
                        return false;
                }
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
