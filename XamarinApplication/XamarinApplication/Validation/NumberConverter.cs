using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XamarinApplication.Validation
{
   public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ValueBetween(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        private bool ValueBetween(object value)
        {
            /* if (value >= 1 && value <= 100)
                 return true;

              if (value == null || ((string)value).Length == 0)
                  return true;*/

            /*if (value is string)
            {
                //int length = ((string)value).Trim().Length;
                //if (length >= 7 && length <= 60)
                if ((int)value >= 1 && (int)value <= 100)
                    return true;
                else
                    return false;
            }

            if (value is string)
            {
                if ((int)value >= 1 && (int)value <= 100)
                return true;
            else
                return false;
            }*/
            return false;

        }
        private bool IsBetween(object val, int low, int high)
        {
            return (int)val > low && (int)val < high;
        }
    }
}
