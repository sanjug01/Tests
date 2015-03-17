
namespace RdClient.Shared.CxWrappers
{
    public enum RadcFeedOperation
    {
        UserInitiatedFeedRefresh = 0,
        DiscoverFeeds = 1,
        ShowConsentUI = 2,
        EditFeeds = 3,
        ShowDemoConsentUI = 4,
        UpdateConsentStatusOnServer = 5,
        SubscribeToFeeds = 6,
        SubscribeToOnPremFeedsCompleted = 7,
        EditFeedsSignOutInitiated = 8,
        EditFeedsSignOutCompleted = 9,
        InitiateCancellation = 10
    }
}
