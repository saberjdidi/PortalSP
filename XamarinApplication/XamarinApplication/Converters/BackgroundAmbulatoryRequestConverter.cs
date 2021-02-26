using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XamarinApplication.Converters
{
    public class BackgroundAmbulatoryRequestConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            int s = (int)value;
            switch (s)
            {
                case 0:
                    return Color.FromHex("#FFFFFF");
                case 1:
                    return Color.FromHex("#4392F1");
            }
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
