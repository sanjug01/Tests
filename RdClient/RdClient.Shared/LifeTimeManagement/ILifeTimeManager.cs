namespace RdClient.Shared.LifeTimeManagement
{
    using System;

    public interface ILifeTimeManager
    {
        event EventHandler<LaunchEventArgs> Launched;
        event EventHandler<SuspendEventArgs> Suspending;
        event EventHandler<ResumeEventArgs> Resuming;

        void OnLaunched(IActivationArgs e);

        void OnSuspending(object sender, ISuspensionArgs e);

        void OnResuming(object sender, IResumingArgs e);
    }
}
