using RdClient.Shared.CxWrappers;
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

    public class SessionModel : ISessionModel
    {
        public event EventHandler<ConnectionCreatedArgs> ConnectionCreated;

        private readonly IRdpConnectionFactory _connectionFactory;

        private readonly ITimerFactory _timerFactory;

        private IRdpConnection _rdpConnection;

        private Snapshotter _snapshotter;
        private List<IRdpCertificate> _acceptedCertificates;

        
        private void EmitConnectionCreated(ConnectionCreatedArgs args)
        {
            if (ConnectionCreated != null)
            {
                ConnectionCreated(this, args);
            }
        }

        public SessionModel(IRdpConnectionFactory connectionFactory, ITimerFactory timerFactory)
        {
            Contract.Requires(connectionFactory != null);
            _connectionFactory = connectionFactory;
            _timerFactory = timerFactory;

            _acceptedCertificates = new List<IRdpCertificate>();
        }

        public void Connect(ConnectionInformation connectionInformation)
        {
            _rdpConnection = _connectionFactory.CreateInstance();
            EmitConnectionCreated(new ConnectionCreatedArgs(_rdpConnection));

            Desktop desktop = connectionInformation.Desktop;
            Credentials credentials = connectionInformation.Credentials;
            IThumbnail thumbnail = connectionInformation.Thumbnail;
            if (thumbnail != null)
            {
                _snapshotter = new Snapshotter(_rdpConnection, thumbnail, _timerFactory);
            }

            RdpPropertyApplier.ApplyDesktop(_rdpConnection as IRdpProperties, desktop);
            _rdpConnection.Connect(credentials, credentials.HaveBeenPersisted);
        }

        public void Disconnect()
        {
            _rdpConnection.Disconnect();

            _rdpConnection = null;
        }
        
        /// <summary>
        ///   Adds certificate to the list of accepted certificate (temporary or persistent)
        /// </summary>
        /// <param name="certificate">presented certificate</param>
        /// <param name="alwaysAccept">indicates if should persist this certificate or not.</param>
        void ISessionModel.AcceptCertificate(IRdpCertificate certificate, bool alwaysAccept)
        {
            Contract.Assert(null != certificate);

            // should add to the datamodel too, only if alwaysAccept
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
    }
}
