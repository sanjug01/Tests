using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace RdClient.Shared.Converters.ErrorLocalizers
{
    public class RdpDisconnectReasonLocalizer : IErrorLocalizer
    {
        public TypeToLocalizedStringConverter TypeToLocalizedStringConverter { private get; set; }

        public string LocalizeError(IRdpError error)
        {
            Contract.Assert(error is RdpDisconnectReason);
            RdpDisconnectReason reason = (RdpDisconnectReason)error;

            string localizedValue;

            localizedValue = this.TypeToLocalizedStringConverter.Convert(reason.Code, typeof(string), null, null) as string;

            if(string.IsNullOrEmpty(localizedValue))
            {
                localizedValue = this.TypeToLocalizedStringConverter.Convert(RdpDisconnectCode.UnknownError, typeof(string), null, null) as string;
            }

            localizedValue += "\n\n";

            if (0 != reason.ULegacyExtendedCode)
            {
                localizedValue += String.Format(
                    this.TypeToLocalizedStringConverter.LocalizedString.GetLocalizedString("Disconnect_ExtendedErrorCode_String"),
                    reason.ULegacyExtendedCode.ToString("X") // format number as hexadecimal
                    );
            }
            else
            {
                localizedValue += String.Format(
                    this.TypeToLocalizedStringConverter.LocalizedString.GetLocalizedString("Disconnect_ErrorCode_String"),
                    reason.ULegacyCode.ToString("X")  // format number as hexadecimal
                    );
            }

            return localizedValue;
        }
    }
}
