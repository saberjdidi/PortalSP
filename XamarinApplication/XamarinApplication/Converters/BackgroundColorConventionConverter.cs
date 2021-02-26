using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XamarinApplication.Converters
{
   public class BackgroundColorConventionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is string && value != null)
            {
                string s = (string)value;
                switch (s)
                {
                    case "EX":
                        return Color.FromHex("#A0A4C9");
                    case "DE":
                        return Color.FromHex("#F8C630");
                    case "AC":
                        return Color.FromHex("#E54F6D");
                    default:
                        return Color.FromHex("#A0A4C9");
                }

            }
            return Color.FromHex("#A0A4C9");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
