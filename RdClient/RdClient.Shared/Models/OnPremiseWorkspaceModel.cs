namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Data;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading.Tasks;

    public enum WorkspaceState
    {
        Subscribed,
        Unsubscribed,
        Subscribing,
        AddingResources,
        Unsubscribing,
        Refreshing     
    }

    [DataContract(IsReference = true)]
    public sealed class OnPremiseWorkspaceModel : SerializableModel
    {
        [DataMember(Name = "CredentialId", EmitDefaultValue=false)]
        private Guid _credId;
        [DataMember(Name = "FeedUrl", EmitDefaultValue=false)]
        private string _feedUrl;
        [DataMember(Name = "State", EmitDefaultValue = false)]
        private WorkspaceState _state;
        [DataMember(Name = "Error", EmitDefaultValue = false)]
        private XPlatError.XResult32 _error;

        private bool _initialized;
        private string _friendlyName;
        private List<RemoteResourceModel> _resources;
        private List<RemoteResourceModel> _tempResources;
        private IRadcClient _client;
        private ApplicationDataModel _dataModel;

        //We need a no-param constructor for deserialization purposes. RadcClient and dataModel are injected through Initialize() method
        public OnPremiseWorkspaceModel()
        {
            _state = WorkspaceState.Unsubscribed;
            _error = XPlatError.XResult32.Succeeded;
            _initialized = false;
        }

        public CredentialsModel Credential
        {
            get
            {
                Guid id = this.CredentialsId;
                if (id != Guid.Empty)
                {
                    return _dataModel.Credentials.GetModel(id);
                }
                else
                {
                    return null;
                }
            }
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

        public List<RemoteResourceModel> Resources
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
                foreach (RemoteResourceModel resource in this.Resources ?? Enumerable.Empty<RemoteResourceModel>())
                {
                    resource.CredentialsId = value;
                }                
            }
        }

        public string FeedUrl
        {
            get { return _feedUrl; }
            set 
            { 
                if (SetProperty(ref _feedUrl, value))
                {
                    _client.FeedUrl = value;
                }
            }
        }

        public string FriendlyName
        {
            get { return _friendlyName; }
            private set { SetProperty(ref _friendlyName, value); }
        }

        public void Initialize(IRadcClient radcClient, ApplicationDataModel dataModel)
        {
            if (_initialized || radcClient == null || dataModel == null)
            {
                throw new InvalidOperationException("Should only initialize once, specifying a non-null radcClient and dataModel");
            }
            _dataModel = dataModel;
            _client = radcClient;
            _client.FeedUrl = this.FeedUrl;
            _client.Events.OperationInProgress += OperationInProgress;
            _client.Events.OperationCompleted += OperationCompleted;
            _client.Events.AddResourcesStarted += AddResourcesStarted;
            _client.Events.AddResourcesFinished += AddResourcesFinished;
            _client.Events.ResourceAdded += ResourceAdded;
            _client.Events.WorkspaceRemoved += WorkspaceRemoved;

        }

        public void TryAndResubscribe()
        {
            if (this.State != WorkspaceState.Subscribed)
            {
                TaskCompletionSource<XPlatError.XResult32> taskSource = new TaskCompletionSource<XPlatError.XResult32>();
                this.UnSubscribe(result => taskSource.SetResult(result));
                taskSource.Task.Wait();
                this.State = WorkspaceState.Unsubscribed;
                taskSource = new TaskCompletionSource<XPlatError.XResult32>();
                this.Subscribe(result => taskSource.SetResult(result));
            }
        }

        public void GetCachedResources(Action<XPlatError.XResult32> completionCallback = null)
        {
            if (this.State != WorkspaceState.Subscribed)
            {
                throw new InvalidOperationException("Must subscribe before getting cached resources");
            }
            this.State = WorkspaceState.Refreshing;
            _client.StartGetCachedFeeds(result => HandleOperationCompleted(result, WorkspaceState.Subscribed, completionCallback));
        }

        public void Refresh(Action<XPlatError.XResult32> completionCallback = null)
        {
            if (this.State == WorkspaceState.Unsubscribed)
            {
                throw new InvalidOperationException("Must subscribe before refreshing");
            }
            this.State = WorkspaceState.Refreshing;
            _client.StartRefreshFeeds(RadcRefreshReason.ManualRefresh, result => HandleOperationCompleted(result, WorkspaceState.Subscribed, completionCallback));
        }

        public void Subscribe(Action<XPlatError.XResult32> completionCallback = null)
        {
            this.State = WorkspaceState.Subscribing;
            _client.StartSubscribeToOnPremFeed(this.FeedUrl, this.Credential, result => HandleOperationCompleted(result, WorkspaceState.Subscribed, completionCallback));
        }

        public void UnSubscribe(Action<XPlatError.XResult32> completionCallback = null)
        {
            this.State = WorkspaceState.Unsubscribing;
            _client.StartRemoveFeed(this.FeedUrl, result => HandleOperationCompleted(result, WorkspaceState.Unsubscribed, completionCallback));
        }

        private void HandleOperationCompleted(XPlatError.XResult32 result, WorkspaceState successState, Action<XPlatError.XResult32> completionCallback)
        {
            this.Error = result;
            if (result == XPlatError.XResult32.Succeeded)
            {
                this.State = successState;
            }
            if (completionCallback != null)
            {
                completionCallback(result);
            }
        }

        private void ResourceAdded(object sender, RadcResourceAddedArgs args)
        {
            Debug.WriteLine("RADC: OnResourceAdded {0}. Feed = {1}", args.FriendlyName, args.FeedUrl);
            this.FriendlyName = args.WorkspaceFriendlyName;
            if (args.ResourceType == RemoteResourceType.OnPremPublishedApp || args.ResourceType == RemoteResourceType.OnPremPublishedDesktop)
            {
                _tempResources.Add(new RemoteResourceModel(args.ResourceId, args.ResourceType, args.FriendlyName, args.RdpFile, args.IconBytes, args.IconWidth, this.CredentialsId));
            }
            else
            {
                throw new InvalidOperationException("Unexpected resource type of " + args.ResourceType.ToString() + " when trying to add a resource to an on prem workspace.");
            }            
        }

        private void WorkspaceRemoved(object sender, RadcWorkspaceRemovedArgs args)
        {
            Debug.WriteLine("RADC: OnRemoveWorkspace URL={0}", args.FeedUrl);            
            this.Resources = new List<RemoteResourceModel>();
            this.State = WorkspaceState.Unsubscribed;            
        }

        private void OperationInProgress(object sender, RadcOperationInProgressArgs args)
        {
            Debug.WriteLine("RADC: OnRadcFeedOperationInProgress {0}", args.OperationType.ToString());
            if (args.OperationType == RadcFeedOperation.InitiateCancellation)
            {
                this.Error = XPlatError.XResult32.Cancelled;              
            }            
        }

        private void OperationCompleted(object sender, RadcOperationCompletedArgs args)
        {
            Debug.WriteLine("RADC: OnRadcFeedOperationCompleted {0}", args.Result.ToString());
        }

        private void AddResourcesFinished(object sender, RadcAddResourcesFinishedArgs args)
        {
            if (this.Error == XPlatError.XResult32.Succeeded)
            {
                this.State = WorkspaceState.Subscribed;
                this.Resources = _tempResources;
            }            
        }

        private void AddResourcesStarted(object sender, RadcAddResourcesStartedArgs args)
        {
            this.State = WorkspaceState.AddingResources;
            _tempResources = new List<RemoteResourceModel>();            
        }
    }
}
