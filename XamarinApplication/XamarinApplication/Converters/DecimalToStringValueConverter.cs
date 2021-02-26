using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XamarinApplication.Converters
{
    /* class DecimalToStringValueConverter : MvxValueConverter<decimal, string>
     {
         protected override string Convert(decimal value, Type targetType, object parameter, CultureInfo culture)
         {
             return $"{value:C2}";
         }
     }*/
    public class DecimalToStringValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is decimal)
                return value.ToString();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal dec;
            if (decimal.TryParse(value as string, out dec))
                return dec;
            return value;
        }
    }
}
