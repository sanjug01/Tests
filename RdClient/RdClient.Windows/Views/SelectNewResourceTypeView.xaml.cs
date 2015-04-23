// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Views
{
    using RdClient.Controls;
    using Windows.UI.Xaml.Controls;

    public sealed partial class SelectNewResourceTypeView : ModalUserControl
    {
        public SelectNewResourceTypeView()
        {
            this.InitializeComponent();
        }

        protected override Control GetFirstTabControl()
        {
            return this.FirstTabControl;
        }
    }
}
