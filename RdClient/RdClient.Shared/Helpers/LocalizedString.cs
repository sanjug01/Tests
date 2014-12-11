﻿using RdClient.Shared.Helpers;
using Windows.ApplicationModel.Resources;

namespace RdClient.Shared.Helpers
{
    public class LocalizedString : IStringTable
    {
        public string GetLocalizedString(string key)
        {
            string result = null;

            result = ResourceLoader.GetForViewIndependentUse().GetString(key);

            if(string.IsNullOrEmpty(result))
            {
                result = key;
            }

            return result;
        }
    }
}