﻿using RdMock;
using System;
using Windows.Foundation;

namespace RdClient.Shared.Test.Mock
{
    public class WindowSize : MockBase, IWindowSize
    {
        public Size Size
        {
            get; set;
        }
    }
}
