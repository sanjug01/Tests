using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;

namespace RdClient.Shared.Models
{
    public class Snapshotter
    {
        public readonly TimeSpan firstSnapshotTime = new TimeSpan(0, 0, 2);
        public readonly TimeSpan snapshotPeriod = new TimeSpan(0, 0, 15);
        private IRdpConnection _connection;
        private IThumbnail _thumbnail;
        private ITimer _timer;
        private GeneralSettings _settings;
        private bool _isConnectionAvailable;

        public Snapshotter(IRdpConnection connection, IThumbnail thumbnail, ITimerFactory timerFactory, GeneralSettings settings)
        {
            _connection = connection;
            _thumbnail = thumbnail;
            _connection.Events.FirstGraphicsUpdate += Events_FirstGraphicsUpdate;
            _connection.Events.ClientDisconnected += Events_ClientDisconnected;
            _connection.Events.ConnectionHealthStateChanged += Events_ConnectionHealthStateChanged;
            _timer = timerFactory.CreateTimer();
            _settings = settings;
            _isConnectionAvailable = true;
        }

        void Events_ConnectionHealthStateChanged(object sender, ConnectionHealthStateChangedArgs e)
        {
            IRdpConnection rdpConnection = sender as IRdpConnection;
            if ((int)RdClientCx.ConnectionHealthState.Warn == e.ConnectionState)
            {
                _isConnectionAvailable = false;
            }
            else if ((int)RdClientCx.ConnectionHealthState.Connected == e.ConnectionState)
            {
                _isConnectionAvailable = true;
            }
        }

        private void Events_FirstGraphicsUpdate(object sender, FirstGraphicsUpdateArgs e)
        {
            _connection.Events.FirstGraphicsUpdate -= Events_FirstGraphicsUpdate; //only expecting this event once
            _timer.Start(this.TakeFirstSnapshot, firstSnapshotTime, false);
        }

        private void Events_ClientDisconnected(object sender, ClientDisconnectedArgs e)
        {
            _connection.Events.ClientDisconnected -= Events_ClientDisconnected; //only expecting this event once
            _timer.Stop();
        }

        private void TakeFirstSnapshot()
        {
            _timer.Stop();
            _timer.Start(this.TakeSnapshot, snapshotPeriod, true);            
            this.TakeSnapshot();       
        }

        private async void TakeSnapshot()
        {
            if (_settings.UseThumbnails && _isConnectionAvailable)
            {
                IRdpScreenSnapshot snapshot = _connection.GetSnapshot();
                //null snapshot means GetSnapshot failed, so skip updating thumbnail.
                if (snapshot != null)
                {
                    await _thumbnail.Update(snapshot);
                }
            }
        }
    }
}
