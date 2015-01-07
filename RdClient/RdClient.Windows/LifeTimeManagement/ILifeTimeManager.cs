namespace RdClient.LifeTimeManagement
{
    using System;

    public interface ILifeTimeManager
    {
        event EventHandler<LaunchEventArgs> Launched;
        event EventHandler<SuspendEventArgs> Suspending;

        void OnLaunched(IActivationArgs e);

        void OnSuspending(object sender, ISuspensionArgs e);
    }
}
