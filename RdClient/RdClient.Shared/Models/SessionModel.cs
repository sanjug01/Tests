using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Data;
using RdClient.Shared.Helpers;
using System;
using System.Diagnostics.Contracts;

namespace RdClient.Shared.Models
{
    public class ConnectionCreatedArgs : EventArgs
    {
        public IRdpConnection RdpConnection { get; private set; }
        public ConnectionCreatedArgs(IRdpConnection rdpConnection)
        {
            RdpConnection = rdpConnection;
        }
    }

    public class ConnectionAutoReconnectingArgs : EventArgs
    {
        public AutoReconnectError DisconnectReason { get; private set; }
        public int AttemptCount { get; private set; }
        public ClientAutoReconnectingContinueDelegate ContinueDelegate { get; private set; }

        public ConnectionAutoReconnectingArgs(ClientAutoReconnectingArgs args)
        {
            if(null != args)
            {
                DisconnectReason = args.DisconnectReason;
                AttemptCount = args.AttemptCount;
                ContinueDelegate = args.ContinueDelegate;
            }
            else
            {
                DisconnectReason = new AutoReconnectError(0); // not used
                AttemptCount = 0; 
                ContinueDelegate = (__continueReconnecting) => {  }; // not used
            }
        }
    }

    public class ConnectionAutoReconnectCompleteArgs : EventArgs
    {
        public ConnectionAutoReconnectCompleteArgs(ClientAutoReconnectCompleteArgs clientAutoReconnectCompleteArgs)
        {
        }
    }

    public class SessionModel : ISessionModel
    {
        public static readonly int MaxReconnectAttempts = 20;

        private readonly IRdpConnectionFactory _connectionFactory;
        private readonly IDeferredExecution _deferredExecution;
        private readonly ICertificateTrust _certificateTrust;

        public event EventHandler<ConnectionCreatedArgs> ConnectionCreated;
        public event EventHandler<ConnectionAutoReconnectingArgs> ConnectionAutoReconnecting;
        public event EventHandler<ConnectionAutoReconnectCompleteArgs> ConnectionAutoReconnectComplete;

        private IRdpConnection _rdpConnection;

        public SessionModel(IRdpConnectionFactory connectionFactory, IDeferredExecution deferredExecution)
        {
            Contract.Requires(connectionFactory != null);
            Contract.Assert(null != deferredExecution);

            _connectionFactory = connectionFactory;
            _deferredExecution = deferredExecution;
            _certificateTrust = new CertificateTrust();
        }
        
        private void EmitConnectionCreated(ConnectionCreatedArgs args)
        {
            _deferredExecution.Defer(() =>
            {
                if (ConnectionCreated != null)
                {
                    ConnectionCreated(this, args);
                }
            });
        }

        private void EmitConnectionAutoReconnecting(ConnectionAutoReconnectingArgs args)
        {
            _deferredExecution.Defer(() =>
            {
                if (ConnectionAutoReconnecting != null)
                {
                    ConnectionAutoReconnecting(this, args);
                }
            });
        }

        private void EmitConnectionAutoReconnectComplete(ConnectionAutoReconnectCompleteArgs args)
        {
            _deferredExecution.Defer(() =>
            {
                if (ConnectionAutoReconnectComplete != null)
                {
                    ConnectionAutoReconnectComplete(this, args);
                }
            });
        }

        public void Connect(ConnectionInformation connectionInformation, ITimerFactory timerFactory, GeneralSettings settings)
        {
            _rdpConnection = _connectionFactory.CreateDesktop();
            EmitConnectionCreated(new ConnectionCreatedArgs(_rdpConnection));

            _rdpConnection.Events.ConnectionHealthStateChanged += HandleConnectionHealthStateChanged;
            _rdpConnection.Events.ClientAutoReconnecting += HandleClientAutoReconnecting;
            _rdpConnection.Events.ClientAutoReconnectComplete += HandleClientAutoReconnectComplete;

            DesktopModel desktop = connectionInformation.Desktop;
            CredentialsModel credentials = connectionInformation.Credentials;

            RdpPropertyApplier.ApplyDesktop(_rdpConnection as IRdpProperties, desktop);
            _rdpConnection.SetLeftHandedMouseMode(desktop.IsSwapMouseButtons);
            _rdpConnection.Connect(credentials, false/* credentials.HaveBeenPersisted TODO: honor the status of the credentials */ );
        }

        public void Disconnect()
        {
            _rdpConnection.Events.ConnectionHealthStateChanged -= HandleConnectionHealthStateChanged;
            _rdpConnection.Events.ClientAutoReconnecting -= HandleClientAutoReconnecting;
            _rdpConnection.Events.ClientAutoReconnectComplete -= HandleClientAutoReconnectComplete;
            _rdpConnection.Disconnect();
            _rdpConnection = null;
        }
        
        /// <summary>
        ///   Adds certificate to the list of accepted certificate (temporary or persistent)
        /// </summary>
        /// <param name="certificate">presented certificate</param>
        void ISessionModel.AcceptCertificate(IRdpCertificate certificate)
        {
            Contract.Assert(null != certificate);
            _certificateTrust.TrustCertificate(certificate);
        }

        /// <summary>
        /// verifies that the presented certificate has been accepted either only for current session or always
        /// </summary>
        /// <param name="certificate">preented certificate</param>
        /// <returns>true if certificates has been accepted</returns>
        bool ISessionModel.IsCertificateAccepted(IRdpCertificate certificate)
        {
            Contract.Assert(null != certificate);
            return _certificateTrust.IsCertificateTrusted(certificate);
        }

        void HandleClientAutoReconnectComplete(object sender, ClientAutoReconnectCompleteArgs e)
        {
            EmitConnectionAutoReconnectComplete(new ConnectionAutoReconnectCompleteArgs(e));
        }

        void HandleClientAutoReconnecting(object sender, ClientAutoReconnectingArgs e)
        {
            if (null != e && e.AttemptCount > MaxReconnectAttempts)
            {
                // stop and complete
                e.ContinueDelegate.Invoke(false);
                EmitConnectionAutoReconnectComplete(null);
            }
            else
            {
                EmitConnectionAutoReconnecting(new ConnectionAutoReconnectingArgs(e));
            }
        }

        void HandleConnectionHealthStateChanged(object sender, ConnectionHealthStateChangedArgs e)
        {
            IRdpConnection rdpConnection = sender as IRdpConnection;
            if ((int)RdClientCx.ConnectionHealthState.Warn == e.ConnectionState)
            {
                this.HandleClientAutoReconnecting(sender, null);
            }
            else if ((int)RdClientCx.ConnectionHealthState.Connected == e.ConnectionState)
            {
                this.HandleClientAutoReconnectComplete(sender, null);
            }
        }
    }
}
