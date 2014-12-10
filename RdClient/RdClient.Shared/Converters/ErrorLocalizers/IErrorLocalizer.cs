using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Helpers;

namespace RdClient.Shared.Converters.ErrorLocalizers
{
    public interface IErrorLocalizer
    {
        IStringTable LocalizedString { set; }
        string LocalizeError(IRdpError error);
    }
}
