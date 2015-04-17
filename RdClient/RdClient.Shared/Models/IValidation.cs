namespace RdClient.Shared.Models
{
    using RdClient.Shared.CxWrappers;

    public interface IValidation
    {
        void Accept();

        void Reject();
    }

    public interface IServerIdentityValidation : IValidation
    {
        DesktopModel Desktop { get; }
    }
}
