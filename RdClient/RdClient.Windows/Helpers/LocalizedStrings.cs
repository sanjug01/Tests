using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace RdClient.Helpers
{
    public class LocalizedStrings : ILocalizedStrings
    {
        public string GetLocalizedString(string key)
        {
            string result = null;

            result = ResourceLoader.GetForViewIndependentUse().GetString(key);


            return result;
        }
    }
}
