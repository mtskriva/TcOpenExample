
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace ukazkaPlc
{
    public class BoolToVisibilityConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool inverted = (parameter is null) ? false : bool.Parse(parameter.ToString());

            if (!inverted)
            {
                if ((bool)value)
                    return Visibility.Visible;
                else
                    return Visibility.Hidden;
            }
            else
                if ((bool)value)
                return Visibility.Hidden;
            else
                return Visibility.Visible;

        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
