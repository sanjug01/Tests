using RdClient.Shared.Helpers;

namespace RdClient.Shared.Test.Mock
{
    public class LocalizedString : ILocalizedString
    {
        public string GetLocalizedString(string key)
        {
            return key;
        }
    }
}
