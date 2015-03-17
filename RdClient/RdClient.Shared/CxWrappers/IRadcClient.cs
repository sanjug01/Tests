namespace RdClient.Shared.CxWrappers
{
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Models;
    using System;

    public enum RadcRefreshReason
    {
        AddRemoteAppAccount = 0,
        EditFeeds = 1,
        ManualRefresh = 2,
        BackgroundRefresh = 3
    }

    public interface IRadcClient
    {
        IRadcEvents Events { get; }

        void StartSubscribeToOnPremFeed(string url, CredentialsModel cred, Action<XPlatError> completionHandler = null);
        void StartRemoveFeed(string url, Action<XPlatError> completionHandler = null);
        void StartGetCachedFeeds(Action<XPlatError> completionHandler = null);
        void StartRefreshFeeds(RadcRefreshReason reason, Action<XPlatError> completionHandler = null);
        void SetBackgroundRefreshInterval(uint minutes);
    }
}
