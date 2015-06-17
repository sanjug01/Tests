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
                ITelemetryEvent sessionTelemetry = telemetryClient.MakeEvent("SessionLaunch");
                ITelemetryEvent sessionDuration = telemetryClient.MakeEvent("SessionDuration");

                sessionSetup.Connection.InitializeSessionTelemetry(sessionSetup.DataModel, sessionTelemetry);
                sessionDuration.AddTag("sourceType", sessionSetup.Connection.TelemetrySourceType);

                return new InactiveSession(monitor, deviceCapabilities, telemetryClient, sessionTelemetry, sessionDuration);
            }

            private InactiveSession(ReaderWriterLockSlim monitor,
                IDeviceCapabilities deviceCapabilities,
                ITelemetryClient telemetryClient,
                ITelemetryEvent sessionTelemetry,
                ITelemetryEvent sessionDuration)
                : base(SessionState.Idle, monitor, deviceCapabilities, telemetryClient, sessionTelemetry, sessionDuration)
            {
                this.SessionTelemetry.AddTag("userInteractionMode", this.DeviceCapabilities.UserInteractionMode);
            }

            public InactiveSession(InternalState otherState) : base(SessionState.Idle, otherState)
            {
            }
        }
    }
}
