using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace XamarinApplication.Converters
{

   public class BackgroundColorRoomConverter : IValueConverter
    { 
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {

        
            bool s = (bool)value;
            switch (s)
            {
                case true:
                    return Color.FromHex("#FB4B4E");
                case false:
                    return Color.FromHex("#0CCA4A");
            }
            return s;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return "";
    }

}
}
