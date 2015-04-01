namespace RdClient.Shared.Converters
{
    using System;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Converter that converts a boolean value to a configured value of the PasswordRevealMode enum.
    /// </summary>
    /// <remarks>The converter may be established in XAML and given the password reveal mode to return
    /// for input boolean values equal to true.</remarks>
    public sealed class BooleanToPasswordRevealModeConverter : IValueConverter
    {
        private PasswordRevealMode _revealMode;

        /// <summary>
        /// Value of the PasswordRevealMode enum returned by the converter for input boolean values equal to true.
        /// </summary>
        /// <remarks>Default reveal mode is PasswordRevealMode.Peek</remarks>
        public PasswordRevealMode RevealMode
        {
            get { return _revealMode; }
            set { _revealMode = value; }
        }

        public BooleanToPasswordRevealModeConverter()
        {
            _revealMode = PasswordRevealMode.Peek;
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (null != targetType && !targetType.Equals(typeof(PasswordRevealMode)))
                throw new ArgumentException("Target type must be PasswordRevealMode", "targetType");

            if(!(value is bool))
                throw new ArgumentException("Value type must be bool", "value");
            //
            // If the input value is true, return the configured value; otherwise, hide the reveal password UI.
            //
            return ((bool)value) ? _revealMode : PasswordRevealMode.Hidden;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
