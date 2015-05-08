using System;
using Windows.ApplicationModel;

namespace RdClient.Shared.LifeTimeManagement
{
    public interface IResumingOperationWrapper
    {
        object Parameters{ get; }
    }
    public interface IResumingArgs
    {
        IResumingOperationWrapper ResumingOperation { get; }
    }
}
