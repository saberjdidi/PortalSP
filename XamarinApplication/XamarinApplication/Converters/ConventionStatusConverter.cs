using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XamarinApplication.Converters
{
   public class ConventionStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is string && value != null)
            {
                string s = (string)value;
                switch (s)
                {
                    case "EX":
                        return "Expired";
                    case "DE":
                        return "Deleted";
                    case "AC":
                        return "Active";
                    default:
                        return "Other";
                }

            }
            return "Other";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
