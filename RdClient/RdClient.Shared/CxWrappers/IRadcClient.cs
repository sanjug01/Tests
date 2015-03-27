﻿namespace RdClient.Shared.CxWrappers
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
        string FeedUrl { get; set; }

        void StartSubscribeToOnPremFeed(string url, CredentialsModel cred, Action<XPlatError.XResult32> completionHandler = null);
        void StartRemoveFeed(string url, Action<XPlatError.XResult32> completionHandler = null);
        void StartGetCachedFeeds(Action<XPlatError.XResult32> completionHandler = null);
        void StartRefreshFeeds(RadcRefreshReason reason, Action<XPlatError.XResult32> completionHandler = null);
        void SetBackgroundRefreshInterval(uint minutes);
    }
}
