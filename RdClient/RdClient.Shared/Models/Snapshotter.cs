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
        private readonly ITimerFactory _timerFactory;
        private ITimer _timer;

        public Snapshotter(IRdpConnection connection, IThumbnail thumbnail, ITimerFactory timerFactory)
        {
            _connection = connection;
            _thumbnail = thumbnail;
            _connection.Events.FirstGraphicsUpdate += Events_FirstGraphicsUpdate;
            _connection.Events.ClientDisconnected += Events_ClientDisconnected;
            _timerFactory = timerFactory;        
        }

        private void Events_FirstGraphicsUpdate(object sender, FirstGraphicsUpdateArgs e)
        {
            _connection.Events.FirstGraphicsUpdate -= Events_FirstGraphicsUpdate; //only expecting this event once
            _timer = _timerFactory.CreateTimer(this.TakeFirstSnapshot, firstSnapshotTime, false);
        }

        private void Events_ClientDisconnected(object sender, ClientDisconnectedArgs e)
        {
            _connection.Events.ClientDisconnected -= Events_ClientDisconnected; //only expecting this event once
            if (_timer != null)
            {
                _timer.Cancel();  
            }
        }

        private void TakeFirstSnapshot()
        {
            this.TakeSnapshot();            
            _timer = _timerFactory.CreateTimer(this.TakeSnapshot, snapshotPeriod, true);            
        }

        private async void TakeSnapshot()
        {
            await _thumbnail.Update(_connection.GetSnapshot());
        }
    }
}
