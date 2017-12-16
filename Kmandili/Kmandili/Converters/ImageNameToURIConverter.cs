using System;
using System.Globalization;
using Xamarin.Forms;

namespace Kmandili.Converters
{
    class ImageNameToUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (App.ServerUrl + "Uploads/" + value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
