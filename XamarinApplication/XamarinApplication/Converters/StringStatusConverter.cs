using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Helpers;

namespace XamarinApplication.Converters
{
    public class StringStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is string && value != null)
            {
                string s = (string)value;
                switch (s)
                {
                    case "ALL":
                        return Languages.ALL;
                    case "CH":
                        return Languages.Checked;
                    case "SV":
                        return Languages.Saved;
                    case "SE":
                        return Languages.Sent;
                    case "TC":
                        return Languages.ToBeCompleted;
                    case "VL":
                        return Languages.Validated;
                    case "SI":
                        return Languages.Signed;
                    case "NS":
                        return Languages.NonSelected;
                    case "":
                        return Languages.None;
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
