﻿using RdClient.Shared.Models;
namespace RdClient.Shared.CxWrappers
{
    public interface IRdpConnectionFactory
    {
        /// <summary>
        /// Create an RDP connection instance representing a remote desktop.
        /// </summary>
        /// <param name="rdpFile">Contents of an RDP file used to initialize the connection. </param>/// 
        /// <returns>New idle RDP connection object.</returns>
        IRdpConnection CreateDesktop(string rdpFile);

        /// <summary>
        /// Create an RDP connection representing a remote application.
        /// </summary>
        /// <param name="rdpFile">Contents of an RDP file used to initialize the connection.</param>
        /// <returns>New idle RDP connection object.</returns>
        /// <remarks>Method calls LaunchRemoteApp with the contents of an RDP file received with a feed.</remarks>
        IRdpConnection CreateApplication(string rdpFile);
    }
}
