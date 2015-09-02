namespace RdClient.Shared.Models
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Windows.Input;

    public class SymbolBarButtonModel : BarButtonModel
    {
        private SegoeGlyph _glyph;
        public SegoeGlyph Glyph
        {
            get { return _glyph; }
            set { SetProperty(ref _glyph, value); }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public SymbolBarButtonModel()
        {
            _glyph = SegoeGlyph.Home;
            _isEnabled = true;
        }
    }
}
