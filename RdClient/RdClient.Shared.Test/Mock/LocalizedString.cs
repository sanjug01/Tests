using RdClient.Shared.Helpers;

namespace RdClient.Shared.Test.Mock
{
    public class LocalizedString : IStringTable
    {
        public string GetLocalizedString(string key)
        {
            // Mock.LocalizedString simply returns the key passed in with a prefix added
            //      the prefix may support up to 2 parameters 
            return "LOCALIZED_"+ "({0}, {1})" + key ;
        }
    }
}
