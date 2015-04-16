namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Models;
    using RdMock;
    using System;

    public class RadcClient : MockBase, IRadcClient
    {
        public IRadcEvents Events { get; set; }

        public string FeedUrl { get; set; }

        public void SetBackgroundRefreshInterval(uint minutes)
        {
            Invoke(new object[] { minutes });
        }

        public void StartGetCachedFeeds(Action<XPlatError.XResult32> completionHandler = null)
        {
            Invoke(new object[] { completionHandler });
        }

        public void StartRefreshFeeds(RadcRefreshReason reason, Action<XPlatError.XResult32> completionHandler = null)
        {
            Invoke(new object[] { reason, completionHandler });
        }

        public void StartRemoveFeed(string url, Action<XPlatError.XResult32> completionHandler = null)
        {
            Invoke(new object[] { url, completionHandler });
        }

        public void StartSubscribeToOnPremFeed(string url, CredentialsModel cred, Action<XPlatError.XResult32> completionHandler = null)
        {
            Invoke(new object[] { url, cred, completionHandler });
        }
    }
}
