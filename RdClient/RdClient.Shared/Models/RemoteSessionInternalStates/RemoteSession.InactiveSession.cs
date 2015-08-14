namespace RdClient.Shared.Models
{
    using RdClient.Shared.Telemetry;
    using System.Threading;

    partial class RemoteSession
    {
        private sealed class InactiveSession : InternalState
        {
            protected override void Activated()
            {
            }

            public static InternalState Create(ReaderWriterLockSlim monitor, IDeviceCapabilities deviceCapabilities, RemoteSessionSetup sessionSetup, ITelemetryClient telemetryClient)
            {
                //
                // Use the connection model to create a session telemetry event that the session will update and report upon completion.
                //
                Telemetry.Events.SessionLaunch sessionLaunch = new Telemetry.Events.SessionLaunch();
                Telemetry.Events.SessionDuration sessionDuration = new Telemetry.Events.SessionDuration(sessionSetup.Connection.TelemetrySourceType);

                sessionSetup.Connection.InitializeSessionTelemetry(sessionSetup.DataModel, sessionLaunch);

                return new InactiveSession(monitor, deviceCapabilities, telemetryClient, sessionLaunch, sessionDuration);
            }

            private InactiveSession(ReaderWriterLockSlim monitor,
                IDeviceCapabilities deviceCapabilities,
                ITelemetryClient telemetryClient,
                Telemetry.Events.SessionLaunch sessionLaunch,
                Telemetry.Events.SessionDuration sessionDuration)
                : base(SessionState.Idle, monitor, deviceCapabilities, telemetryClient, sessionLaunch, sessionDuration)
            {
                this.SessionLaunch.userInteractionMode = this.DeviceCapabilities.UserInteractionModeLabel;
            }

            public InactiveSession(InternalState otherState) : base(SessionState.Idle, otherState)
            {
            }
        }
    }
}
