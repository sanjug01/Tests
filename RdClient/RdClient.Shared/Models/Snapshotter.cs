﻿using RdClient.Shared.CxWrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using RdClient.Shared.Helpers;

namespace RdClient.Shared.Models
{
    public class Snapshotter
    {
        public readonly TimeSpan firstSnapshotTime = new TimeSpan(0, 0, 1);
        public readonly TimeSpan snapshotPeriod = new TimeSpan(0, 0, 15);
        private IRdpConnection _connection;
        private IThumbnail _thumbnail;
        private ITimerFactory _timerFactory;
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
            _timer = _timerFactory.CreateTimer(this.TakeFirstSnapshot, firstSnapshotTime, false);
        }

        private void Events_ClientDisconnected(object sender, ClientDisconnectedArgs e)
        {
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
