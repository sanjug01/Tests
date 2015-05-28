using RdClient.Shared.Helpers;
using Windows.ApplicationModel.Resources;

namespace RdClient.Shared.Helpers
{
    public class LocalizedString : IStringTable
    {
        public string GetLocalizedString(string key)
        {
            return ResourceLoader.GetForViewIndependentUse().GetString(key) ?? string.Empty;
        }
    }
}
