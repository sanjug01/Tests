using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.CxWrappers
{
    public enum RadcRefreshReason
    {
        AddRemoteAppAccount = 0,
        EditFeeds = 1,
        ManualRefresh = 2,
        BackgroundRefresh = 3
    }

    public interface IRadcClient
    {
        void StartSubscribeToOnPremFeed(string url, CredentialsModel cred, Action<XPlatError> completionHandler = null);
        void StartRemoveFeed(string url, Action<XPlatError> completionHandler = null);
        void StartGetCachedFeeds(Action<XPlatError> completionHandler = null);
        void StartRefreshFeeds(RadcRefreshReason reason, Action<XPlatError> completionHandler = null);
        void SetBackgroundRefreshInterval(uint minutes);
    }
}
