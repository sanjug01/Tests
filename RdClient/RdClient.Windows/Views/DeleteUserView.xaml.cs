namespace RdClient.Views
{
    using RdClient.Controls;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;

    public sealed partial class DeleteUserView : ModalUserControl, IPresentableView
    {
        public DeleteUserView()
        {
            this.InitializeComponent();
        }
    }
}
