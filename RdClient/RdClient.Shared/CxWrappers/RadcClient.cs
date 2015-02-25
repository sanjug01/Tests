using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.CxWrappers
{
    public class RadcClient
    {
        private readonly RdClientCx.RadcClient _client;

        public RadcClient()
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

            _client.SubscribeToOnPremFeed(@"https://ts-wlbs-a.ntdev.corp.microsoft.com/RDWeb/feed/webfeed.aspx", @"ntdev\texas", ".Gold-finger007");
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
            throw new NotImplementedException();
        }

        void _client_OnRemoveWorkspace(string strFeedURL)
        {
            throw new NotImplementedException();
        }

        void _client_OnRadcFeedOperationInProgress(RdClientCx.RadcFeedOperation feedOperation)
        {
            throw new NotImplementedException();
        }

        void _client_OnRadcFeedOperationCompleted(RdClientCx.RadcErrorCode errorCode)
        {
            throw new NotImplementedException();
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
