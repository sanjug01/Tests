using RdClient.Shared.Helpers;

namespace RdClient.Shared.Test.Mock
{
    public class LocalizedString : IStringTable
    {
        public string GetLocalizedString(string key)
        {
            return "LOCALIZED_" + key;
        }
    }
}
