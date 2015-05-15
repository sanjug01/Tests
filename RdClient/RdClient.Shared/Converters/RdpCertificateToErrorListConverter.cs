using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace RdClient.Shared.Converters
{
    public sealed class RdpCertificateToErrorListConverter : IValueConverter
    {
        public TypeToLocalizedStringConverter TypeToLocalizedStringConverter {private get; set;}

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IRdpCertificate cert = value as IRdpCertificate;
            if (TypeToLocalizedStringConverter == null)
            {
                throw new InvalidOperationException("LocalizedString property must be set before Convert is called");
            }     
            if (cert == null || cert.Error == null)
            {
                throw new ArgumentException("value to convert must be a non-null IRdpCertificate with a non-null Error property");
            }
            else
            {               
                IList<string> errorList = new List<string>();
                foreach(CertificateError err in Enum.GetValues(typeof(CertificateError)))
                {
                    if(CertificateErrorHelper.ErrorContainsFlag(cert.Error.ErrorFlags, err))
                    {
                        errorList.Add(this.TypeToLocalizedStringConverter.Convert(err, typeof(string), parameter, language) as string);
                    }
                }

                return errorList;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new InvalidOperationException("ConvertBack not supported");
        }
    }
}
