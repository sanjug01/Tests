using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;

namespace RdClient.Shared.Models
{
    public class Snapshotter
    {
        private static readonly TimeSpan FirstSnapshotTime = new TimeSpan(0, 0, 2);
        private static readonly TimeSpan SnapshotPeriod = new TimeSpan(0, 0, 15);

        private readonly IRdpConnection _connection;
        private readonly IThumbnailEncoder _thumbnail;
        private readonly ITimer _timer;
        private GeneralSettings _settings;
        private bool _isConnectionAvailable;

        public Snapshotter(IRdpConnection connection, IThumbnailEncoder thumbnail, ITimerFactory timerFactory, GeneralSettings settings)
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
            _timer.Start(this.TakeFirstSnapshot, FirstSnapshotTime, false);
        }

        private void Events_ClientDisconnected(object sender, ClientDisconnectedArgs e)
        {
            _connection.Events.ClientDisconnected -= Events_ClientDisconnected; //only expecting this event once
            _timer.Stop();
        }

        private void TakeFirstSnapshot()
        {
            _timer.Stop();
            _timer.Start(this.TakeSnapshot, SnapshotPeriod, true);            
            this.TakeSnapshot();       
        }

        private void TakeSnapshot()
        {
            if (_settings.UseThumbnails && _isConnectionAvailable)
            {
                IRdpScreenSnapshot snapshot = _connection.GetSnapshot();
                //
                // null snapshot means GetSnapshot failed, so skip updating thumbnail.
                //
                if (snapshot != null)
                {
                    //
                    // Send received snapshot data to the thumbnail encoder; the encoder will resample the image,
                    // compress it for serialization and emit an event with the compressed bytes.
                    // DesktopViewModel will receive the event and update its Thumbnail property on the UI thread, which will
                    // trigger an update of the screenshot image.
                    //
                    _thumbnail.Update(snapshot);
                }
            }
        }
    }
}
