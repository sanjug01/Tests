using RdClient.Shared.Helpers;
using Windows.ApplicationModel.Resources;

namespace RdClient.Shared.Helpers
{
    public class LocalizedString : ILocalizedString
    {
        public string GetLocalizedString(string key)
        {
            string result = null;

            result = ResourceLoader.GetForViewIndependentUse().GetString(key);

            return result;
        }
    }
}
