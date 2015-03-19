namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Data;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    public enum WorkspaceState
    {
        Ok,
        Unsubscribed,
        Subscribing,
        AddingResources,
        Unsubscribing,
        Refreshing,
        Error        
    }

    [DataContract(IsReference = true)]
    public sealed class OnPremiseWorkspaceModel : SerializableModel
    {
        private WorkspaceState _state;
        private Guid _credId;
        private string _feedUrl;
        private string _friendlyName;
        private List<RemoteConnectionModel> _resources;
        private List<RemoteConnectionModel> _tempResources;
        private RadcClient _client;

        public OnPremiseWorkspaceModel(RadcClient radcClient)
        {
            _state = WorkspaceState.Unsubscribed;
            _credId = Guid.Empty;
            _feedUrl = "";
            _friendlyName = "";
            _resources = new List<RemoteConnectionModel>();
            _client = radcClient;
            _client.Events.OperationInProgress += OperationInProgress;
            _client.Events.OperationCompleted += OperationCompleted;
            _client.Events.AddResourcesStarted += AddResourcesStarted;
            _client.Events.AddResourcesFinished += AddResourcesFinished;
            _client.Events.ResourceAdded += ResourceAdded;
            _client.Events.WorkspaceRemoved += WorkspaceRemoved;
        }

        public WorkspaceState State
        {
            get { return _state; }
            private set { SetProperty(ref _state, value); }
        }

        public List<RemoteConnectionModel> Resources
        {
            get { return _resources; }
            private set { SetProperty(ref _resources, value); }
        }

        public Guid CredentialsId
        {
            get { return _credId; }
            set { SetProperty(ref _credId, value); }
        }

        public string FeedUrl
        {
            get { return _feedUrl; }
            set { SetProperty(ref _feedUrl, value); }
        }

        public string FriendlyName
        {
            get { return _friendlyName; }
            private set { SetProperty(ref _friendlyName, value); }
        }

        public void Refresh()
        {
            if (this.State != WorkspaceState.Ok && this.State != WorkspaceState.Unsubscribed && this.State != WorkspaceState.Error)
            {
                throw new InvalidOperationException("Can only refresh when workspace is subscribed and has no operations pending");
            }
            this.State = WorkspaceState.Refreshing;
            _client.StartRefreshFeeds(RadcRefreshReason.ManualRefresh, result =>
            {
                if (result == XPlatError.XResult32.Succeeded)
                {
                    this.State = WorkspaceState.Ok;
                }
                else
                {
                    this.State = WorkspaceState.Error;
                }
            });
        }

        public void Subscribe()
        {
            if (this.State != WorkspaceState.Ok && this.State != WorkspaceState.Unsubscribed && this.State != WorkspaceState.Error)
            {
                throw new InvalidOperationException("Can only subscribe when workspace has no operations pending");
            }
            this.State = WorkspaceState.Subscribing;
            _client.StartSubscribeToOnPremFeed(this.FeedUrl, new CredentialsModel() { Username = @"rdvteam\tstestuser1", Password = @"1234AbCd" }, result =>
            {
                if (result == XPlatError.XResult32.Succeeded)
                {
                    this.State = WorkspaceState.Ok;
                }
                else
                {
                    this.State = WorkspaceState.Error;
                }
            });         
        }

        public void UnSubscribe()
        {
            if (this.State != WorkspaceState.Ok || this.State != WorkspaceState.Error)
            {
                throw new InvalidOperationException("Can only subscribe when workspace has no operations pending");
            }
            this.State = WorkspaceState.Subscribing;
            _client.StartRemoveFeed(this.FeedUrl, result =>
            {
                if (result == XPlatError.XResult32.Succeeded)
                {
                    this.State = WorkspaceState.Unsubscribed;
                }
                else
                {
                    this.State = WorkspaceState.Error;
                }
            });   
        }


        private void ResourceAdded(object sender, RadcResourceAddedArgs args)
        {
            Debug.WriteLine("RADC: OnResourceAdded {0}. Feed = {1}", args.FriendlyName, args.FeedUrl);
            if (args.FeedUrl.Equals(this.FeedUrl))
            {
                if (!String.IsNullOrWhiteSpace(args.WorkspaceFriendlyName))
                {
                    this.FriendlyName = args.WorkspaceFriendlyName;
                }                
                if (args.ResourceType == RemoteResourceType.OnPremPublishedApp)
                {
                    _tempResources.Add(new RemoteApplicationModel(args.ResourceId, args.FriendlyName, args.RdpFile, args.IconBytes, args.IconWidth));
                }
                else if (args.ResourceType == RemoteResourceType.OnPremPublishedDesktop)
                {
                    DesktopModel desktop = new DesktopModel();
                    desktop.RdpFile = args.RdpFile;
                    desktop.CredentialsId = this.CredentialsId;
                    _tempResources.Add(desktop);
                }
                else
                {
                    throw new InvalidOperationException("Unexpected resource type of " + args.ResourceType.ToString() + " when trying to add a resource to an on prem workspace.");
                }
            }
        }

        private void WorkspaceRemoved(object sender, RadcWorkspaceRemovedArgs args)
        {
            Debug.WriteLine("RADC: OnRemoveWorkspace URL={0}", args.FeedUrl);
            if (_feedUrl.Equals(args.FeedUrl))
            {
                this.Resources.Clear();
                this.State = WorkspaceState.Unsubscribed;
            }
        }

        private void OperationInProgress(object sender, RadcOperationInProgressArgs args)
        {
            if (args.OperationType == RadcFeedOperation.InitiateCancellation)
            {

            }
            Debug.WriteLine("RADC: OnRadcFeedOperationInProgress {0}", args.OperationType.ToString());
        }

        private void OperationCompleted(object sender, RadcOperationCompletedArgs args)
        {
            Debug.WriteLine("RADC: OnRadcFeedOperationCompleted {0}", args.Result.ToString());
        }

        private void AddResourcesFinished(object sender, RadcAddResourcesFinishedArgs args)
        {
            if (this.FeedUrl.Equals(args.FeedUrl))
            {
                this.Resources = _tempResources;
            }
        }

        private void AddResourcesStarted(object sender, RadcAddResourcesStartedArgs args)
        {
            if (this.FeedUrl.Equals(args.FeedUrl))
            {
                _tempResources = new List<RemoteConnectionModel>();
            }
        }
    }
}
