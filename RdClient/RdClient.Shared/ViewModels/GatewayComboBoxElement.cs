namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    public enum GatewayComboBoxType
    {
        AddNew = 0,
        None = 1,
        Gateway = 2
    }

    public sealed class GatewayComboBoxOrder : IComparer<GatewayComboBoxElement>
    {
        public int Compare(GatewayComboBoxElement x, GatewayComboBoxElement y)
        {
            Contract.Assert(null != x);
            Contract.Assert(null != y);

            int compareType = x.GatewayComboBoxType < y.GatewayComboBoxType ? -1 : x.GatewayComboBoxType > y.GatewayComboBoxType ? 1 : 0;

            if (0 == compareType && GatewayComboBoxType.Gateway == x.GatewayComboBoxType)
            {
                // compare by gateway hostname if both have gateways
                Contract.Assert(null != x.Gateway && null != x.Gateway.Model);
                Contract.Assert(null != y.Gateway && null != y.Gateway.Model);

                return string.Compare(x.Gateway.Model.HostName, y.Gateway.Model.HostName);
            }

            return compareType;
        }
    }

    public class GatewayComboBoxElement: MutableObject
    {
        private readonly IModelContainer<GatewayModel> _gateway;

        public IModelContainer<GatewayModel> Gateway { get { return _gateway; } }

        private readonly GatewayComboBoxType _gatewayComboBoxType;
        public GatewayComboBoxType GatewayComboBoxType { get { return _gatewayComboBoxType; } }

        public GatewayComboBoxElement(GatewayComboBoxType gatewayComboBoxType, IModelContainer<GatewayModel> gateway = null)
        {            
            _gatewayComboBoxType  = gatewayComboBoxType;
            _gateway = gateway;
        }
    }
}
