using RdClient.Shared.Converters.ErrorLocalizers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Windows.UI.Xaml.Data;

namespace RdClient.Converters
{
    public class RdpErrorToLocalizedStringConverter : IValueConverter
    {
        private Dictionary<string, Type> _localizerMap = new Dictionary<string,Type>();

        private IStringTable _localizedString;
        public IStringTable LocalizedString { set { _localizedString = value; } }

        public RdpErrorToLocalizedStringConverter()
        {
            _localizerMap["RdpDisconnectReason"] = typeof(RdpDisconnectReasonLocalizer);
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Requires(value is IRdpError);
            Contract.Requires(targetType.Equals(typeof(string)));

            IRdpError error = (IRdpError)value;

            if(_localizerMap.ContainsKey(error.Category))
            {
                IErrorLocalizer localizer = Activator.CreateInstance(_localizerMap[error.Category]) as IErrorLocalizer;
                localizer.LocalizedString = this._localizedString;
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
