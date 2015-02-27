using System;
using System.Diagnostics.Contracts;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace RdClient.Converters
{
    using RdClient.Shared.Input.ZoomPan;

    public class PanKnobStateToForegroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Requires(targetType.Equals(typeof(Brush)));

            if (!(value is PanKnobState))
            {
                throw new ArgumentException("Invalid value type, expected PanKnobState", "value");
            }

            PanKnobState state = (PanKnobState)value;

            switch (state)
            {
                case PanKnobState.Inactive:
                    // white
                    return new SolidColorBrush(Color.FromArgb(127, 255, 255, 255));                    
                case PanKnobState.Active:
                    // black
                    return new SolidColorBrush(Color.FromArgb(127, 0, 0, 0));
                case PanKnobState.Moving:
                    // black
                    return new SolidColorBrush(Color.FromArgb(127, 255, 255, 255));
            }

            return null;
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
