namespace RdClient.Shared.Navigation
{
    public interface IPresentationCompletion
    {
        void Completed(IPresentableView view, object result);
    }
}
