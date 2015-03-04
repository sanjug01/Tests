namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    [DataContract(IsReference = true)]
    public sealed class OnPremiseWorkspaceModel : SerializableModel
    {
        private readonly RdClientCx.RadcClient _client;

        private HashSet<string> _resources;

        public OnPremiseWorkspaceModel()
        {
            _resources = new HashSet<string>();

            int xRes = RdClientCx.RadcClient.GetInstance(out _client);
            RdTrace.IfFailXResultThrow(xRes, "RdClientCx.RadcClient.GetInstance() failed");

            //Add handlers
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

            Task<int> subscribeTask = Task.Factory.StartNew<int>(() => { return _client.SubscribeToOnPremFeed(@"https://ts-wlbs-a.ntdev.corp.microsoft.com/RDWeb/feed/webfeed.aspx", @"ntdev\texas", ".Gold-finger007"); });
            RdTrace.IfFailXResultThrow(subscribeTask.Result, "RdClientCx.RadcClient.SubscribeToOnPremFeed() failed");

            xRes = _client.RefreshFeedResources(RdClientCx.RefreshFeedReason.ManualRefresh);
            RdTrace.IfFailXResultThrow(xRes, "RdClientCx.RadcClient.RefreshFeedResources() failed");
        }

        void _client_OnShowDemoConsentPage(RdClientCx.OnShowDemoConsentPageCompletedHandler spShowDemoConsentPageCompletedHandler, bool fFirstTimeSignIn)
        {
            throw new NotImplementedException();
        }

        void _client_OnShowAzureSignOutDialog()
        {
            throw new NotImplementedException();
        }

        void _client_OnShowAppInvites(RdClientCx.OnShowAppInvitesCompletedHandler spOnShowAppInvitesCompletedHandler, RdClientCx.ConsentStatusInfo[] spConsentStatusInfoList, bool fFirstTimeSignIn)
        {
            throw new NotImplementedException();
        }

        void _client_OnResourceAdded(string strWorksapceLocalId, string strWorkspaceFriendlyName, string strFeedURL, string strResourceId, string strResourceFriendlyName, string strRdpFile, RdClientCx.ResourceType resouceType, byte[] spIcon, uint iconWidth)
        {
            if (!_resources.Contains(strResourceId))
            {
                _resources.Add(strResourceId);
            }
        }

        void _client_OnRemoveWorkspace(string strFeedURL)
        {
            throw new NotImplementedException();
        }

        void _client_OnRadcFeedOperationInProgress(RdClientCx.RadcFeedOperation feedOperation)
        {
            RdTrace.TraceDbg(string.Format("RADC: OnRadcFeedOperationInProgress {0}", feedOperation.ToString()));
        }

        void _client_OnRadcFeedOperationCompleted(RdClientCx.RadcErrorCode errorCode)
        {
            RdTrace.TraceDbg(string.Format("RADC: OnRadcFeedOperationCompleted {0}", errorCode.ToString()));
        }

        void _client_OnHideEditAppInvitesUI()
        {
            throw new NotImplementedException();
        }

        void _client_OnEditAppInvites(RdClientCx.OnShowAppInvitesCompletedHandler spOnShowAppInvitesCompletedHandler, RdClientCx.ConsentStatusInfo[] spConsentStatusInfoList)
        {
            throw new NotImplementedException();
        }

        void _client_OnEndResourcesAdded(string strFeedURL)
        {
            throw new NotImplementedException();
        }

        void _client_OnBeginResourcesAdded(string strFeedURL)
        {
            throw new NotImplementedException();
        }

        void _client_OnAzureSignOutCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
