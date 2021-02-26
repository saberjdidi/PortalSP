using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace XamarinApplication.Validation
{
    public class FiscalCodeCorrectToHideLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return isFiscalCodeCorrect(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private bool isFiscalCodeCorrect(object value)
        {
            if (value == null || ((string)value).Length == 0)
                return true;

            if (value is string)
            {
                bool isEmail = Regex.IsMatch(
                    //(string)value, "^[a-z0-9._-]+@[a-z0-9._-]+\\.[a-z]{2,6}$");
                    (string)value, "^([A-Za-z]{6}[0-9lmnpqrstuvLMNPQRSTUV]{2}[abcdehlmprstABCDEHLMPRST]{1}[0-9lmnpqrstuvLMNPQRSTUV]{2}[A-Za-z]{1}[0-9lmnpqrstuvLMNPQRSTUV]{3}[A-Z]{1}$)|(([sS]{1}[tT]{1}[pP]{1})([0-9]{13}))$");

                int length = ((string)value).Trim().Length;
                //if (length >= 7 && length <= 60 && isEmail)
                if (isEmail)
                    return true;
                else
                    return false;

            }
            return false;
        }
    }
}
