namespace RdClient.Shared.ValidationRules
{
    using RdClient.Shared.Models;
    using System;

    // allow to compare a Model object with an object of another type (e.g string), 
    // for validation
    public interface IModelEqualityComparer<T1,T2>
    {
        bool Equals(T1 x, T2 y);
    }

    public class CredentialsEqualityComparer : IModelEqualityComparer<CredentialsModel, string>
    {
        public bool Equals(CredentialsModel model, string value)
        {
            return StringComparer.CurrentCultureIgnoreCase.Equals(model.Username, value);
        }
    }

    public class GatewayEqualityComparer : IModelEqualityComparer<GatewayModel, string>
    {
        public bool Equals(GatewayModel model, string value)
        {
            return StringComparer.CurrentCultureIgnoreCase.Equals(model.HostName, value);
        }
    }
}
