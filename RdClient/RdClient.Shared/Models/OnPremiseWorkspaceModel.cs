namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Data;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.Serialization;

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
        private XPlatError.XResult32 _error;
        private Guid _credId;
        private string _feedUrl;
        private string _friendlyName;
        private List<RemoteConnectionModel> _resources;
        private List<RemoteConnectionModel> _tempResources;
        private RadcClient _client;
        private ApplicationDataModel _dataModel;

        public OnPremiseWorkspaceModel()
        {
            throw new NotImplementedException();
        }

        public OnPremiseWorkspaceModel(RadcClient radcClient, ApplicationDataModel dataModel)
        {
            _dataModel = dataModel;
            _state = WorkspaceState.Unsubscribed;
            _error = XPlatError.XResult32.Succeeded;
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

        public CredentialsModel Credential
        {
            get { return _dataModel.LocalWorkspace.Credentials.GetModel(this.CredentialsId); }
        }

        public WorkspaceState State
        {
            get { return _state; }
            private set { SetProperty(ref _state, value); }
        }

        public XPlatError.XResult32 Error
        {
            get { return _error; }
            private set { SetProperty(ref _error, value); }
        }

        public List<RemoteConnectionModel> Resources
        {
            get { return _resources; }
            private set { SetProperty(ref _resources, value); }
        }

        public Guid CredentialsId
        {
            get 
            { 
                return _credId; 
            }
            set 
            { 
                SetProperty(ref _credId, value);
                if (this.Resources != null)
                {
                    foreach (RemoteConnectionModel connection in this.Resources)
                    {
                        DesktopModel desktop = connection as DesktopModel;
                        RemoteApplicationModel app = connection as RemoteApplicationModel;
                        if (desktop != null)
                        {
                            desktop.CredentialsId = value;
                        }
                        else if (app != null)
                        {
                            app.CredentialId = value;
                        }
                    }
                }
            }
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

        public void GetCachedResources(Action<XPlatError.XResult32> completionCallback = null)
        {
            this.State = WorkspaceState.Refreshing;
            _client.StartGetCachedFeeds(result => HandleOperationCompleted(result, WorkspaceState.Ok, completionCallback));
        }

        public void Refresh(Action<XPlatError.XResult32> completionCallback = null)
        {
            if (this.State != WorkspaceState.Ok && this.State != WorkspaceState.Unsubscribed && this.State != WorkspaceState.Error)
            {
                throw new InvalidOperationException("Can only refresh when workspace is subscribed and has no operations pending");
            }
            this.State = WorkspaceState.Refreshing;
            _client.StartRefreshFeeds(RadcRefreshReason.ManualRefresh, result => HandleOperationCompleted(result, WorkspaceState.Ok, completionCallback));
        }

        public void Subscribe(Action<XPlatError.XResult32> completionCallback = null)
        {
            if (this.State != WorkspaceState.Ok && this.State != WorkspaceState.Unsubscribed && this.State != WorkspaceState.Error)
            {
                throw new InvalidOperationException("Can only subscribe when workspace has no operations pending");
            }
            this.State = WorkspaceState.Subscribing;
            _client.StartSubscribeToOnPremFeed(this.FeedUrl, this.Credential, result => HandleOperationCompleted(result, WorkspaceState.Ok, completionCallback));
        }

        public void UnSubscribe(Action<XPlatError.XResult32> completionCallback = null)
        {
            if (this.State != WorkspaceState.Ok && this.State != WorkspaceState.Error)
            {
                throw new InvalidOperationException("Can only subscribe when workspace has no operations pending");
            }
            this.State = WorkspaceState.Unsubscribing;
            _client.StartRemoveFeed(this.FeedUrl, result => HandleOperationCompleted(result, WorkspaceState.Unsubscribed, completionCallback));
        }

        private void HandleOperationCompleted(XPlatError.XResult32 result, WorkspaceState successState, Action<XPlatError.XResult32> completionCallback)
        {
            this.Error = result;
            this.State = (result == XPlatError.XResult32.Succeeded) ? successState : WorkspaceState.Error;
            if (completionCallback != null)
            {
                completionCallback(result);
            }
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
                    _tempResources.Add(new RemoteApplicationModel(args.ResourceId, args.FriendlyName, args.RdpFile, args.IconBytes, args.IconWidth, this.CredentialsId));
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
                _dataModel.OnPremWorkspaces.Remove(this);
                _client.Events.OperationInProgress -= OperationInProgress;
                _client.Events.OperationCompleted -= OperationCompleted;
                _client.Events.AddResourcesStarted -= AddResourcesStarted;
                _client.Events.AddResourcesFinished -= AddResourcesFinished;
                _client.Events.ResourceAdded -= ResourceAdded;
                _client.Events.WorkspaceRemoved -= WorkspaceRemoved;
            }
        }

        private void OperationInProgress(object sender, RadcOperationInProgressArgs args)
        {
            if (args.OperationType == RadcFeedOperation.InitiateCancellation)
            {
                this.Error = XPlatError.XResult32.Cancelled;
                this.State = WorkspaceState.Error;              
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
                if (this.Error == XPlatError.XResult32.Succeeded)
                {
                    this.State = WorkspaceState.Ok;
                    this.Resources = _tempResources;
                }
                else
                {
                    this.State = WorkspaceState.Error;
                }
            }
        }

        private void AddResourcesStarted(object sender, RadcAddResourcesStartedArgs args)
        {
            if (this.FeedUrl.Equals(args.FeedUrl))
            {
                this.State = WorkspaceState.AddingResources;
                _tempResources = new List<RemoteConnectionModel>();
            }
        }
    }
}
