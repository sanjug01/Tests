namespace RdClient.Shared.ViewModels
{
    /// <summary>
    /// Interface of a generic event that can be handled.
    /// </summary>
    public interface IHandleable
    {
        bool Handled
        {
            get;
            set;
        }
    }
}
