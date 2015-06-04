namespace RdClient.Shared.ViewModels
{
    public interface IBellyBandViewModel
    {
        /// <summary>
        /// method to allow the belly band dialogs to dismissed externaly
        ///  for example, on pressing back.
        /// </summary>
        void Terminate();
    }
}
