﻿using System;
using Windows.ApplicationModel;

namespace RdClient.Shared.LifeTimeManagement
{
    // The reason why we need a custom interface here is that 
    // In the OnSuspending Event we receive a SuspendingEventArgs parameter which is
    // a public sealed class with no constructor. This makes mocking pretty difficult.
    // The way we work around it is by having a wrapper interface which contains a member for
    // each member in the original interface with a matching type. When OnSuspending gets invoked,
    // we construct instances of the wrapper interfaces and copy the members one by one.

    public interface ISuspendingOperationWrapper
    {
        DateTimeOffset Deadline { get; }

        ISuspendingDeferral Deferral { get; }
    }
    public interface ISuspensionArgs
    {
        ISuspendingOperationWrapper SuspendingOperation { get; }
    }
}