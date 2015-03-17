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

        public RadcClient(IRadcEventSource eventProxy, IDeferredExecution deferredExecution)
        {
            int xRes = RdClientCx.RadcClient.GetInstance(out _client);
            RdTrace.IfFailXResultThrow(xRes, "RdClientCx.RadcClient.GetInstance() failed");
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
            _eventProxy = eventProxy;
            _deferredExecution = deferredExecution;
        }

        public IRadcEvents Events
        {
            get 
            { 
                return _deferredExecution as IRadcEvents; 
            }
        }

        public void StartSubscribeToOnPremFeed(string url, Models.CredentialsModel cred, Action<XPlatError.XResult32> completionHandler = null)
        {
            _deferredExecution.Defer(() =>
                {
                    int xres = _client.SubscribeToOnPremFeed(url, cred.Username, cred.Password);
                    Errors.XPlatError.XResult32 error = RdpTypeConverter.ConvertFromCx(xres);
                    if (completionHandler != null)
                    {
                        completionHandler(error);
                    }
                });
        }

        public void StartRemoveFeed(string url, Action<XPlatError.XResult32> completionHandler = null)
        {
            _deferredExecution.Defer(() =>
            {
                int xres = _client.RemoveFeed(url);
                Errors.XPlatError.XResult32 error = RdpTypeConverter.ConvertFromCx(xres);
                if (completionHandler != null)
                {
                    completionHandler(error);
                }
            });
        }

        public void StartGetCachedFeeds(Action<XPlatError.XResult32> completionHandler = null)
        {
            _deferredExecution.Defer(() =>
            {
                int xres = _client.GetCachedFeedResources();
                Errors.XPlatError.XResult32 error = RdpTypeConverter.ConvertFromCx(xres);
                if (completionHandler != null)
                {
                    completionHandler(error);
                }
            });
        }

        public void StartRefreshFeeds(RadcRefreshReason reason, Action<XPlatError.XResult32> completionHandler = null)
        {
            _deferredExecution.Defer(() =>
            {
                int xres = _client.RefreshFeedResources(RdpTypeConverter.ConvertToCx(reason));
                Errors.XPlatError.XResult32 error = RdpTypeConverter.ConvertFromCx(xres);
                if (completionHandler != null)
                {
                    completionHandler(error);
                }
            });
        }

        public void SetBackgroundRefreshInterval(uint minutes)
        {
            _deferredExecution.Defer(() =>
            {
                int xres = _client.SetBackgroundRefreshIntervalMinute(minutes);
                RdTrace.IfFailXResultThrow(xres, "Unexpected failure in RadcClient.SetBackgroundRefreshInterval()");
            });
        }

        private void _client_OnResourceAdded(string strWorksapceLocalId, string strWorkspaceFriendlyName, string strFeedURL, string strResourceId, string strResourceFriendlyName, string strRdpFile, RdClientCx.ResourceType resouceType, byte[] spIcon, uint iconWidth)
        {
            var args = new RadcResourceAddedArgs();
            _eventProxy.EmitResourceAdded(this, args);
        }

        private void _client_OnRemoveWorkspace(string strFeedURL)
        {
            var args = new RadcWorkspaceRemovedArgs();
            _eventProxy.EmitWorkspaceRemoved(this, args);
        }

        private void _client_OnRadcFeedOperationInProgress(RdClientCx.RadcFeedOperation feedOperation)
        {
            var args = new RadcOperationInProgressArgs();
            _eventProxy.EmitOperationInProgress(this, args);
        }

        private void _client_OnRadcFeedOperationCompleted(RdClientCx.RadcErrorCode errorCode)
        {
            var args = new RadcOperationCompletedArgs();
            _eventProxy.EmitOperationCompleted(this, args);
        }

        private void _client_OnEndResourcesAdded(string strFeedURL)
        {
            var args = new RadcAddResourcesFinishedArgs();
            _eventProxy.EmitAddResourcesFinished(this, args);
        }

        private void _client_OnBeginResourcesAdded(string strFeedURL)
        {
            var args = new RadcAddResourcesStartedArgs();
            _eventProxy.EmitAddResourcesStarted(this, args);
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
