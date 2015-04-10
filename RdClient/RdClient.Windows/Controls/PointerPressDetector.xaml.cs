// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace RdClient.Controls
{
    using System.Windows.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;

    /// <summary>
    /// Translucent overlay that reports pointer down events to a bindable ICommand object.
    /// The control invokes the command set to its property and passes the PointerRoutedEventArgs object
    /// to the command as the argument.
    /// </summary>
    /// <remarks>The control is designed to cover parts of the UI outside of presented views
    /// to detect that user has interacted with the UI outside of the view to dismiss the view.</remarks>
    public sealed partial class PointerPressDetector : UserControl
    {
        public readonly DependencyProperty PointerPressDetectedProperty = DependencyProperty.Register("PointerPressDetected",
            typeof(ICommand), typeof(PointerPressDetector),
            new PropertyMetadata(null));

        public PointerPressDetector()
        {
            this.InitializeComponent();
        }

        public ICommand PointerPressDetected
        {
            get { return (ICommand)GetValue(PointerPressDetectedProperty); }
            set { SetValue(PointerPressDetectedProperty, value); }
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            ICommand command = this.PointerPressDetected;

            base.OnPointerPressed(e);
            //
            // If the command is there and it can be executed, execute it. Do not mark the event as handled
            // so it bubbles up the visual tree and some underlying UI element will have a chance to handle
            // the pointer press.
            //
            if(!e.Handled && null != command && command.CanExecute(e))
            {
                command.Execute(e);
            }
        }
    }
}
