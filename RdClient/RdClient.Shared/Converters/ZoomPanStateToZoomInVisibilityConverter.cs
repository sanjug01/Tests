using System;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace RdClient.Converters
{
    using RdClient.Shared.Input.ZoomPan;
    public class ZoomPanStateToZoomInVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Requires(targetType.Equals(typeof(Visibility)));

            if (!(value is ZoomPanState))
            {
                throw new ArgumentException("Invalid value type, expected ZoomPanState", "value");
            }

            ZoomPanState state = (ZoomPanState)value;

            if (state == ZoomPanState.TouchMode_MinScale)
            {
                    return Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            //
            // There is no reverse translation; the converter can be used only one-way.
            //
            throw new InvalidOperationException("ConvertBack not supported");
        }
    }
}
