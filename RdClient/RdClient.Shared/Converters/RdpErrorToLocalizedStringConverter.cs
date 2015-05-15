using RdClient.Shared.Converters.ErrorLocalizers;
using RdClient.Shared.CxWrappers.Errors;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml.Data;

namespace RdClient.Shared.Converters
{
    public class RdpErrorToLocalizedStringConverter : IValueConverter
    {
        private Dictionary<Type, Type> _localizerMap = new Dictionary<Type, Type>();

        public TypeToLocalizedStringConverter TypeToLocalizedStringConverter { private get; set; }

        public RdpErrorToLocalizedStringConverter()
        {
            _localizerMap[typeof(RdpDisconnectReason)] = typeof(RdpDisconnectReasonLocalizer);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Requires(value is IRdpError);
            Contract.Requires(targetType.Equals(typeof(string)));

            IRdpError error = (IRdpError)value;

            if(_localizerMap.ContainsKey(error.GetType()))
            {
                IErrorLocalizer localizer = Activator.CreateInstance(_localizerMap[error.GetType()]) as IErrorLocalizer;
                localizer.TypeToLocalizedStringConverter = this.TypeToLocalizedStringConverter;
                return localizer.LocalizeError(error);
            }
            else
            {
                return error.ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
