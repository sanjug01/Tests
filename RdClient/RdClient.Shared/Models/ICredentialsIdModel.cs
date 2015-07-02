using System;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Telemetry;

namespace RdClient.Shared.Models
{
    public interface ICredentialsIdModel
    {
        Guid CredentialsId { get; }
        bool HasCredentials { get; }
    }
}