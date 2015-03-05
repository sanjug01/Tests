namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    [DataContract(IsReference = true)]
    public sealed class OnPremiseWorkspaceModel : SerializableModel
    {
        private RdClientCx.RadcClient _client;
        private Task _worker;
        private CredentialsModel _creds;
        private HashSet<RemoteApplicationModel> _resources;

        public HashSet<RemoteApplicationModel> Resources
        {
            get { return _resources; }
        }

        public CredentialsModel Credentials
        {
            get { return _creds; }
        }

        public OnPremiseWorkspaceModel()
        {            
            _resources = new HashSet<RemoteApplicationModel>();

            _worker = Task.Factory.StartNew(() =>
                {
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

                    string feedUrl = @"https://es-vm2k12r2.rdvteam.stbtest.microsoft.com/rdweb/feed/webfeed.aspx";
                    _creds = new CredentialsModel() { Username = @"rdvteam\tstestuser1", Password = @"1234AbCd" };
                   
                    xRes = _client.RemoveFeed(feedUrl);
                    xRes = _client.SubscribeToOnPremFeed(feedUrl, _creds.Username, _creds.Password);                    
                    //xRes = _client.RefreshFeedResources(RdClientCx.RefreshFeedReason.ManualRefresh);
                    //xRes = _client.GetCachedFeedResources();
                });
            _worker.Wait();
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
            Debug.WriteLine("RADC: OnResourceAdded {0}", strResourceFriendlyName);
            if (resouceType == RdClientCx.ResourceType.OnPremPublishedApp)
            {
                _resources.Add(new RemoteApplicationModel(strResourceId, strResourceFriendlyName, strRdpFile, spIcon, iconWidth));
            }
        }

        void _client_OnRemoveWorkspace(string strFeedURL)
        {
            Debug.WriteLine("RADC: OnRemoveWorkspace URL={0}", strFeedURL);
            _resources.Clear();
        }

        void _client_OnRadcFeedOperationInProgress(RdClientCx.RadcFeedOperation feedOperation)
        {
            Debug.WriteLine("RADC: OnRadcFeedOperationInProgress {0}", feedOperation.ToString());
        }

        void _client_OnRadcFeedOperationCompleted(RdClientCx.RadcErrorCode errorCode)
        {
            Debug.WriteLine("RADC: OnRadcFeedOperationCompleted {0}", errorCode.ToString());
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
            Debug.WriteLine("RADC: OnEndResourcesAdded URL={0}", strFeedURL);
        }

        void _client_OnBeginResourcesAdded(string strFeedURL)
        {
            Debug.WriteLine("RADC: OnBeginResourcesAdded URL={0}", strFeedURL);
        }

        void _client_OnAzureSignOutCompleted()
        {
            throw new NotImplementedException();
        }
    }
}
