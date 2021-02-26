using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XamarinApplication.Converters
{
   public class BackgroundNomenclatureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {


            bool s = (bool)value;
            switch (s)
            {
                case true:
                    return Color.FromHex("#2EC4B6");
                case false:
                    return Color.FromHex("#E71D36");
            }
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }

    }
}
