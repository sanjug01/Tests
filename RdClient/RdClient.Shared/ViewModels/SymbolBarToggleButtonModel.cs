namespace RdClient.Shared.ViewModels
{
    public sealed class SymbolBarToggleButtonModel : SymbolBarButtonModel
    {
        private bool _isChecked;

        public SymbolBarToggleButtonModel()
        {
            _isChecked = false;
        }

        public bool IsChecked
        {
            get { return _isChecked; }
            set { SetProperty(ref _isChecked, value); }
        }
    }
}
