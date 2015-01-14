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

        public Snapshotter(IRdpConnection connection, IThumbnail thumbnail, ITimerFactory timerFactory, GeneralSettings settings)
        {
            _connection = connection;
            _thumbnail = thumbnail;
            _connection.Events.FirstGraphicsUpdate += Events_FirstGraphicsUpdate;
            _connection.Events.ClientDisconnected += Events_ClientDisconnected;
            _timer = timerFactory.CreateTimer();
            _settings = settings;
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
            if (_settings.UseThumbnails)
            {
                // TODO : this cause AccessViolation if connection is lost/reconnecting- see bug 1341660			
                await _thumbnail.Update(_connection.GetSnapshot());
            }
        }
    }
}
