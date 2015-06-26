namespace RdClient.Shared.Models
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Windows.Input;

    public class SymbolBarButtonModel : BarButtonModel
    {
        private SegoeGlyph _glyph;

        public SymbolBarButtonModel()
        {
            _glyph = SegoeGlyph.Home;
        }

        public SegoeGlyph Glyph
        {
            get { return _glyph; }
            set { SetProperty(ref _glyph, value); }
        }
    }
}
