namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Helpers;
    using System;

    public class Snapshotter : MutableObject
    {
        private static readonly TimeSpan FirstSnapshotTime = new TimeSpan(0, 0, 5);
        private static readonly TimeSpan SnapshotPeriod = new TimeSpan(0, 0, 15);

        private readonly IRdpConnection _connection;
        private readonly IRdpEvents _events;
        private readonly IThumbnailEncoder _encoder;
        private readonly ITimer _timer;
        private GeneralSettings _settings;
        private bool _isConnectionAvailable;

        public Snapshotter(IRdpConnection connection,
            IRdpEvents events,
            IThumbnailEncoder encoder,
            ITimerFactory timerFactory,
            GeneralSettings settings)
        {
            _connection = connection;
            _events = events;
            _encoder = encoder;
            _timer = timerFactory.CreateTimer();
            _settings = settings;
            _isConnectionAvailable = true;
        }

        public void Activate()
        {
            _events.FirstGraphicsUpdate += OnFirstGraphicsUpdate;
            _events.ConnectionHealthStateChanged += OnConnectionHealthStateChanged;
        }

        public void Deactivate()
        {
            using(LockWrite())
                _timer.Stop();
            _events.FirstGraphicsUpdate -= OnFirstGraphicsUpdate;
            _events.ConnectionHealthStateChanged -= OnConnectionHealthStateChanged;
        }

        private void OnConnectionHealthStateChanged(object sender, ConnectionHealthStateChangedArgs e)
        {
            IRdpConnection rdpConnection = sender as IRdpConnection;

            using (LockWrite())
            {
                if ((int)RdClientCx.ConnectionHealthState.Connected == e.ConnectionState)
                {
                    _isConnectionAvailable = true;
                }
                else
                {
                    _isConnectionAvailable = false;
                }
            }
        }

        private void OnFirstGraphicsUpdate(object sender, FirstGraphicsUpdateArgs e)
        {
            _events.FirstGraphicsUpdate -= OnFirstGraphicsUpdate; // only expecting this event once

            using(LockWrite())
                _timer.Start(this.TakeFirstSnapshot, FirstSnapshotTime, false);
        }

        private void TakeFirstSnapshot()
        {
            using (LockWrite())
            {
                _timer.Stop();
                _timer.Start(this.TakeSnapshot, SnapshotPeriod, true);
                this.TakeSnapshot();
            }
        }

        private void TakeSnapshot()
        {
            using (LockRead())
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
                        _encoder.Update(snapshot);
                    }
                }
            }
        }
    }
}
