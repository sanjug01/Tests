namespace RdClient.Converters
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Diagnostics.Contracts;
    using System.Collections.Generic;
    using Windows.UI.Xaml.Data;

    public sealed class RtfDocumentTypeToLocalizedTitleStringConverter : IValueConverter
    {
        private static readonly Dictionary<InternalDocType, string> _codeMap;

        private IStringTable _localizedString;

        static RtfDocumentTypeToLocalizedTitleStringConverter()
        {
            _codeMap = new Dictionary<InternalDocType, string>();
            _codeMap[InternalDocType.EulaDoc] = "RtfDocument_Eula_Title_String";
            _codeMap[InternalDocType.HelpDoc] = "RtfDocument_HelpDoc_Title_String";
            _codeMap[InternalDocType.PrivacyDoc] = "RtfDocument_PrivacyDoc_Title_String";
            _codeMap[InternalDocType.ThirdPartyNotices] = "RtfDocument_ThirdPartyNotices_Title_String";
        }

        public IStringTable LocalizedString { set { _localizedString = value; } }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Requires(value is bool);
            Contract.Requires(targetType.Equals(typeof(string)));

            InternalDocType docType = (InternalDocType) value;
            if (_localizedString == null)
            {
                throw new InvalidOperationException("LocalizedString property must be set before Convert is called");
            }
            else
            {
                string result;
                if (_codeMap.ContainsKey(docType))
                {
                    result = _localizedString.GetLocalizedString(_codeMap[docType]);
                }
                else
                {
                    result = "";
                }
                return result;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("ConvertBack not supported");
        }
    }
}
