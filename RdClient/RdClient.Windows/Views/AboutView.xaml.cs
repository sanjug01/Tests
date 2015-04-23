// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Views
{
    using RdClient.Controls;
    using RdClient.Navigation;
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;
    using Windows.UI.Xaml.Controls;

    public sealed partial class AboutView : ModalUserControl
    {
        public AboutView()
        {
            this.InitializeComponent();
        }

        protected override Control GetFirstTabControl()
        {
            return this.CloseHyperlink;
        }
    }
}
