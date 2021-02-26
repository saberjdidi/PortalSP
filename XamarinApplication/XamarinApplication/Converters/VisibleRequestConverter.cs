using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XamarinApplication.Converters
{
   public class VisibleRequestConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string && value != null)
            {
                string s = (string)value;
                switch (s)
                {
                    case "CH":
                        return true;
                    case "SV":
                        return false;
                    case "SE":
                        return true;
                    case "TC":
                        return false;
                    case "VL":
                        return true;
                    case "SI":
                        return true;
                    case "NS":
                        return false;
                    default:
                        return true;
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
