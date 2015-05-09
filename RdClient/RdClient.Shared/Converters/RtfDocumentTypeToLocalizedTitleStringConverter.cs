namespace RdClient.Converters
{
    using RdClient.Shared.Converters;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Diagnostics.Contracts;
    using System.Collections.Generic;
    using Windows.UI.Xaml.Data;

    public sealed class RtfDocumentTypeToLocalizedTitleStringConverter : IValueConverter
    {
        public TypeToLocalizedStringConverter TypeToLocalizedStringConverter { private get; set; }
        

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Requires(value is InternalDocType);
            Contract.Requires(targetType.Equals(typeof(string)));
            InternalDocType docType = (InternalDocType)value;

            if (TypeToLocalizedStringConverter == null)
            {
                throw new InvalidOperationException("LocalizedString property must be set before Convert is called");
            }

            return this.TypeToLocalizedStringConverter.Convert(docType, typeof(string), parameter, language) as string;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("ConvertBack not supported");
        }
    }
}
