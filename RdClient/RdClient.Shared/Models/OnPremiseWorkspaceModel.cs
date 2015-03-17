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

    [DataContract(IsReference = true)]
    public sealed class OnPremiseWorkspaceModel : SerializableModel
    {
        private RadcClient _client;
        private CredentialsModel _creds;
        private string _feedUrl;
        private HashSet<RemoteApplicationModel> _resources;

        public OnPremiseWorkspaceModel(string feedUrl)
        {            
            _resources = new HashSet<RemoteApplicationModel>();
            _client = new RadcClient(new RadcEventSource(), new Helpers.TaskExecutor());
            _client.Events.OperationInProgress += OperationInProgress;
            _client.Events.OperationCompleted += OperationCompleted;
            _client.Events.AddResourcesStarted += AddResourcesStarted;
            _client.Events.AddResourcesFinished += AddResourcesFinished;
            _client.Events.ResourceAdded += ResourceAdded;
            _client.Events.WorkspaceRemoved += WorkspaceRemoved;            
            _feedUrl = feedUrl;
            _creds = new CredentialsModel() { Username = @"rdvteam\tstestuser1", Password = @"1234AbCd" };
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
            _client.StartRefreshFeeds(
                RadcRefreshReason.ManualRefresh,
                refResult =>
                {
                    if (refResult == XPlatError.XResult32.Succeeded)
                    {
                        tcs.SetResult(true);
                    }
                    else
                    {
                        _client.StartSubscribeToOnPremFeed(_feedUrl, _creds, subResult =>
                            {
                                tcs.SetResult(true);
                            });
                    }
                });
            tcs.Task.Wait();
        }

        public HashSet<RemoteApplicationModel> Resources
        {
            get { return _resources; }
        }

        public CredentialsModel Credentials
        {
            get { return _creds; }
            set { SetProperty(ref _creds, value); }
        }

        public string FeedUrl
        {
            get { return _feedUrl; }
        }

        private void ResourceAdded(object sender, RadcResourceAddedArgs args)
        {
            Debug.WriteLine("RADC: OnResourceAdded {0}", args.FriendlyName);
            if (args.ResourceType == RemoteResourceType.OnPremPublishedApp)
            {
                _resources.Add(new RemoteApplicationModel(args.ResourceId, args.FriendlyName, args.RdpFile, args.IconBytes, args.IconWidth));
            }
        }

        private void WorkspaceRemoved(object sender, RadcWorkspaceRemovedArgs args)
        {
            Debug.WriteLine("RADC: OnRemoveWorkspace URL={0}", args.FeedUrl);
            if (_feedUrl.Equals(args.FeedUrl))
            {
                _resources.Clear();
            }
        }

        private void OperationInProgress(object sender, RadcOperationInProgressArgs args)
        {
            Debug.WriteLine("RADC: OnRadcFeedOperationInProgress {0}", args.OperationType.ToString());
        }

        private void OperationCompleted(object sender, RadcOperationCompletedArgs args)
        {
            Debug.WriteLine("RADC: OnRadcFeedOperationCompleted {0}", args.Result.ToString());
        }

        private void AddResourcesFinished(object sender, RadcAddResourcesFinishedArgs args)
        {
            Debug.WriteLine("RADC: OnEndResourcesAdded URL={0}", args.FeedUrl);
        }

        private void AddResourcesStarted(object sender, RadcAddResourcesStartedArgs args)
        {
            Debug.WriteLine("RADC: OnBeginResourcesAdded URL={0}", args.FeedUrl);
        }
    }
}
