namespace RdClient.Shared.CxWrappers
{
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;
    using System;

    public class RadcClient : IRadcClient
    {
        private RdClientCx.RadcClient _client;
        private IRadcEventSource _eventProxy;
        private IDeferredExecution _deferredExecution;
        private bool _operationInProgress;
        private string _feedUrl;

        public RadcClient(IRadcEventSource eventProxy, IDeferredExecution deferredExecution)
        {
            _eventProxy = eventProxy;
            _deferredExecution = deferredExecution;
            _operationInProgress = false;
            Func<int> radcCall = () => { return RdClientCx.RadcClient.GetInstance(out _client); };
            Action<XPlatError.XResult32> completionHandler = (result) =>
            {
                _client.OnAzureSignOutCompleted += _client_OnAzureSignOutCompleted;
                _client.OnBeginResourcesAdded += _client_OnBeginResourcesAdded;
                _client.OnEditAppInvites += _client_OnEditAppInvites;
                _client.OnEndResourcesAdded += _client_OnEndResourcesAdded;
                _client.OnHideEditAppInvitesUI += _client_OnHideEditAppInvitesUI;
                _client.OnRadcFeedOperationCompleted += _client_OnRadcFeedOperationCompleted;
                _client.OnRadcFeedOperationInProgress += _client_OnRadcFeedOperationInProgress;
                _client.OnRemoveWorkspace += _client_OnRemoveWorkspace;
                _client.OnResourceAdded += _client_OnResourceAdded;
                _client.OnShowAppInvites += _client_OnShowAppInvites;
                _client.OnShowAzureSignOutDialog += _client_OnShowAzureSignOutDialog;
                _client.OnShowDemoConsentPage += _client_OnShowDemoConsentPage;
            };
            CallCxRadcClient(radcCall, completionHandler);
        }

        public string FeedUrl
        {
            get { return _feedUrl; }
            set { _feedUrl = value; }
        }

        public IRadcEvents Events
        {
            get { return _eventProxy as IRadcEvents; }
        }

        public void StartSubscribeToOnPremFeed(string url, Models.CredentialsModel cred, Action<XPlatError.XResult32> completionHandler = null)
        {
            Func<int> radcCall = () => { return _client.SubscribeToOnPremFeed(url, cred.Username, cred.Password); };
            CallCxRadcClient(radcCall, completionHandler);
        }

        public void StartRemoveFeed(string url, Action<XPlatError.XResult32> completionHandler = null)
        {
            Func<int> radcCall = () => { return _client.RemoveFeed(url); };
            CallCxRadcClient(radcCall, completionHandler);
        }

        public void StartGetCachedFeeds(Action<XPlatError.XResult32> completionHandler = null)
        {
            Func<int> radcCall = () => { return _client.GetCachedFeedResources(); };
            CallCxRadcClient(radcCall, completionHandler);
        }

        public void StartRefreshFeeds(RadcRefreshReason reason, Action<XPlatError.XResult32> completionHandler = null)
        {
            Func<int> radcCall = () => { return _client.RefreshFeedResources(RdpTypeConverter.ConvertToCx(reason)); };
            CallCxRadcClient(radcCall, completionHandler);
        }

        public void SetBackgroundRefreshInterval(uint minutes)
        {
            Func<int> radcCall = () => { return _client.SetBackgroundRefreshIntervalMinute(minutes); };
            CallCxRadcClient(radcCall, null);
        }

        private void CallCxRadcClient(Func<int> radcCall, Action<XPlatError.XResult32> completionHandler)
        {
            _deferredExecution.Defer(() =>
            {
                XPlatError.XResult32 result = XPlatError.XResult32.Failed;
                try
                {
                    _operationInProgress = true;
                    int xres = radcCall();
                    result = RdpTypeConverter.ConvertFromCx(xres);
                }
                catch (Exception e)
                {
                    //log exception and let completionHandler handle the failure
                    RdTrace.TraceDbg("Call to Cx.RadcClient threw exception" + e.Message);
                }
                finally
                {
                    _operationInProgress = false;
                    if (completionHandler != null)
                    {
                        completionHandler(result);
                    }
                }
            });
        }

        //Only forward this event if an action on this RadcClient instance triggered it, or is for this feedURL (RadcClient only allows a single workspace per feedURL, so we can treat it as an ID)
        private void HandleCxRadcEvent(Action action, string feedUrl = null)
        {
            if (_operationInProgress || string.Compare(feedUrl, this.FeedUrl, StringComparison.OrdinalIgnoreCase) == 0)
            {
                action();
            }
        }

        private void _client_OnResourceAdded(string workspaceId, string workspaceFriendlyName, string feedUrl, string resourceId, string friendlyName, string rdpFile, RdClientCx.ResourceType resourceType, byte[] iconBytes, uint iconWidth)
        {
            HandleCxRadcEvent(() =>
            {
                var args = new RadcResourceAddedArgs(workspaceId, workspaceFriendlyName, feedUrl, resourceId, friendlyName, rdpFile, RdpTypeConverter.ConvertFromCx(resourceType), iconBytes, iconWidth);
                _eventProxy.EmitResourceAdded(this, args);
            }, feedUrl);
        }

        private void _client_OnRemoveWorkspace(string feedUrl)
        {
            HandleCxRadcEvent(() =>
            {
                var args = new RadcWorkspaceRemovedArgs(feedUrl);
                _eventProxy.EmitWorkspaceRemoved(this, args);
            }, feedUrl);
        }

        private void _client_OnRadcFeedOperationInProgress(RdClientCx.RadcFeedOperation feedOperation)
        {
            HandleCxRadcEvent(() =>
            {
                var args = new RadcOperationInProgressArgs(RdpTypeConverter.ConvertFromCx(feedOperation));
                _eventProxy.EmitOperationInProgress(this, args);
            });
        }

        private void _client_OnRadcFeedOperationCompleted(RdClientCx.RadcErrorCode errorCode)
        {
            HandleCxRadcEvent(() =>
            {
                var args = new RadcOperationCompletedArgs(RdpTypeConverter.ConvertFromCx(errorCode));
                _eventProxy.EmitOperationCompleted(this, args);
            });
        }

        private void _client_OnEndResourcesAdded(string feedUrl)
        {
            HandleCxRadcEvent(() =>
            {
                var args = new RadcAddResourcesFinishedArgs(feedUrl);
                _eventProxy.EmitAddResourcesFinished(this, args);
            }, feedUrl);
        }

        private void _client_OnBeginResourcesAdded(string feedUrl)
        {
            HandleCxRadcEvent(() =>
            {
                var args = new RadcAddResourcesStartedArgs(feedUrl);
                _eventProxy.EmitAddResourcesStarted(this, args);
            }, feedUrl);
        }

        private void _client_OnShowDemoConsentPage(RdClientCx.OnShowDemoConsentPageCompletedHandler spShowDemoConsentPageCompletedHandler, bool fFirstTimeSignIn)
        {
            throw new NotImplementedException();
        }

        private void _client_OnShowAzureSignOutDialog()
        {
            throw new NotImplementedException();
        }

        private void _client_OnShowAppInvites(RdClientCx.OnShowAppInvitesCompletedHandler spOnShowAppInvitesCompletedHandler, RdClientCx.ConsentStatusInfo[] spConsentStatusInfoList, bool fFirstTimeSignIn)
        {
            throw new NotImplementedException();
        }

        private void _client_OnHideEditAppInvitesUI()
        {
            throw new NotImplementedException();
        }

        private void _client_OnEditAppInvites(RdClientCx.OnShowAppInvitesCompletedHandler spOnShowAppInvitesCompletedHandler, RdClientCx.ConsentStatusInfo[] spConsentStatusInfoList)
        {
            throw new NotImplementedException();
        }

        private void _client_OnAzureSignOutCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
