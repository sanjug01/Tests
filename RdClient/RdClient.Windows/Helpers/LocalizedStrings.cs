using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;

namespace RdClient.Helpers
{
    public class LocalizedStrings : ILocalizedStrings
    {
        public string GetLocalizedString(string key)
        {
            return ResourceLoader.GetForCurrentView().GetString(key);
        }
    }
}
