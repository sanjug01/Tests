using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.CxWrappers.Utils;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

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
        private const int MaxReconnectAttempts = 20;
        public event EventHandler<ConnectionCreatedArgs> ConnectionCreated;
        public event EventHandler<ConnectionAutoReconnectingArgs> ConnectionAutoReconnecting;
        public event EventHandler<ConnectionAutoReconnectCompleteArgs> ConnectionAutoReconnectComplete;

        private readonly IRdpConnectionFactory _connectionFactory;
        private IRdpConnection _rdpConnection;
        private List<IRdpCertificate> _acceptedCertificates;
        
        private void EmitConnectionCreated(ConnectionCreatedArgs args)
        {
            if (ConnectionCreated != null)
            {
                ConnectionCreated(this, args);
            }
        }

        private void EmitConnectionAutoReconnecting(ConnectionAutoReconnectingArgs args)
        {
            if (ConnectionAutoReconnecting != null)
            {
                ConnectionAutoReconnecting(this, args);
            }
        }

        private void EmitConnectionAutoReconnectComplete(ConnectionAutoReconnectCompleteArgs args)
        {
            if (ConnectionAutoReconnectComplete != null)
            {
                ConnectionAutoReconnectComplete(this, args);
            }
        }

        public SessionModel(IRdpConnectionFactory connectionFactory)
        {
            Contract.Requires(connectionFactory != null);
            _connectionFactory = connectionFactory;
            _acceptedCertificates = new List<IRdpCertificate>();
        }

        public void Connect(ConnectionInformation connectionInformation, ITimerFactory timerFactory, GeneralSettings settings)
        {
            _rdpConnection = _connectionFactory.CreateInstance();
            EmitConnectionCreated(new ConnectionCreatedArgs(_rdpConnection));

            _rdpConnection.Events.ConnectionHealthStateChanged += HandleConnectionHealthStateChanged;
            _rdpConnection.Events.ClientAutoReconnecting += HandleClientAutoReconnecting;
            _rdpConnection.Events.ClientAutoReconnectComplete += HandleClientAutoReconnectComplete;

            Desktop desktop = connectionInformation.Desktop;
            Credentials credentials = connectionInformation.Credentials;
            IThumbnail thumbnail = connectionInformation.Thumbnail;
            if (thumbnail != null)
            {
                Snapshotter snapshotter = new Snapshotter(_rdpConnection, thumbnail, timerFactory, settings);
            }

            RdpPropertyApplier.ApplyDesktop(_rdpConnection as IRdpProperties, desktop);
            _rdpConnection.SetLeftHandedMouseMode(desktop.IsSwapMouseButtons);
            _rdpConnection.Connect(credentials, credentials.HaveBeenPersisted);
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
            _acceptedCertificates.Add(certificate);            
        }

        /// <summary>
        /// verifies that the presented certificate has been accepted either only for current session or always
        /// </summary>
        /// <param name="certificate">preented certificate</param>
        /// <returns>true if certificates has been accepted</returns>
        bool ISessionModel.IsCertificateAccepted(IRdpCertificate certificate)
        {
            Contract.Assert(null != certificate);
            foreach(IRdpCertificate accepterCertificate in _acceptedCertificates)
            {
                // compare serial numbers
                if (certificate.SerialNumber.SequenceEqual<byte>(accepterCertificate.SerialNumber))
                {
                    return true;
                }
            }
            return false;
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
