using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XamarinApplication.Converters
{
    public class DateSearchConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is string && value != null)
            {
                string s = (string)value;
                switch (s)
                {
                    case "request_creation_date":
                        return "Creation Date";
                    case "request_check_date":
                        return "Check Date";
                    case "request_validation_date":
                        return "Validation Date";
                }

            }
            return "Creation Date";

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
