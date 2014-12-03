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

namespace RdClient.Shared.Models
{
    class Snapshotter
    {
        private readonly TimeSpan snapShotPeriod = new TimeSpan(0, 0, 15);
        private IRdpConnection _connection;
        private Thumbnail _thumbnail;
        private ThreadPoolTimer _timer;

        public Snapshotter(IRdpConnection connection, Thumbnail thumbnail)
        {
            _connection = connection;
            _thumbnail = thumbnail;
            _connection.Events.FirstGraphicsUpdate += Events_FirstGraphicsUpdate;
            _connection.Events.ClientDisconnected += Events_ClientDisconnected;                       
        }

        private async void Events_FirstGraphicsUpdate(object sender, FirstGraphicsUpdateArgs e)
        {
            await TakeSnapshot();
            _timer = ThreadPoolTimer.CreatePeriodicTimer(async (timer) => await TakeSnapshot(), snapShotPeriod);
        }

        private void Events_ClientDisconnected(object sender, ClientDisconnectedArgs e)
        {
            if (_timer != null)
            {
                _timer.Cancel();               
            }
        }

        private async Task TakeSnapshot()
        {
            int width, height;
            byte[] bytes;
            _connection.GetSnapshot(out width, out height, out bytes);
            await _thumbnail.Update((uint)width, (uint)height, bytes);
        }
    }
}
