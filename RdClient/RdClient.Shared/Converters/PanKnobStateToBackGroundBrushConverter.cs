namespace RdClient.Converters
{
    using RdClient.Shared.Input.ZoomPan;
    using System;
    using Windows.UI;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;

    public class PanKnobStateToBackgroundBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is PanKnobState))
            {
                throw new ArgumentException("Invalid value type, expected PanKnobState", "value");
            }

            PanKnobState state = (PanKnobState)value;

            switch (state)
            {
                case PanKnobState.Inactive:
                    // gray/black
                    return new SolidColorBrush(Color.FromArgb(127, 0, 0, 0));                    
                case PanKnobState.Active:
                    // blue
                    return new SolidColorBrush(Color.FromArgb(127, 0, 0, 255));
                case PanKnobState.Moving:
                    // blue
                    return new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
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
