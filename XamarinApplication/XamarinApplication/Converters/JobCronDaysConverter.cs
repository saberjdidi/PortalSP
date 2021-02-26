using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Helpers;

namespace XamarinApplication.Converters
{
   public class JobCronDaysConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            if (value is string && value != null)
            {
                string s = (string)value;
                switch (s)
                {
                    case "MON":
                        return Languages.Monday;
                    case "TUE":
                        return Languages.Tuesday;
                    case "WED":
                        return Languages.Wednesday;
                    case "THU":
                        return Languages.Thursday;
                    case "FRI":
                        return Languages.Friday;
                    case "SAT":
                        return Languages.Saturday;
                    case "SUN":
                        return Languages.Sunday;
                }

            }
            return Languages.Monday;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
