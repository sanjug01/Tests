﻿namespace RdClient.Shared.Models
{
    using System.Runtime.Serialization;

    [DataContract(IsReference = true)]
    public sealed class RemoteApplicationModel : RemoteConnectionModel
    {
        public RemoteApplicationModel()
        {
        }
    }
}