namespace RdClient.Shared.ViewModels
{
    using System.Windows.Input;

    /// <summary>
    /// Subset of glyphs for use with the Segoe MDL2 Assets font.
    /// http://osgdesigntools/iconfontviewer/
    /// </summary>
    public enum SegoeGlyph
    {
        Help = 0xE897,
        Delete = 0xE74D,
        Home = 0xE80F,
        Forward = 0xE72A,
        Back = 0xE72B,
        Settings = 0xE713,
        Search = 0xE721,
        People = 0xE716,
        Add = 0xE710,
        More = 0xE712,
        Edit = 0xE70F,
        Zoom = 0xE71E,
        ZoomIn = 0xE8A3,
        ZoomOut = 0xE71F,
        MultiSelection = 0xE762,
        Keyboard =0xE765,
        EnterFullScreen = 0xE740,
        ExitFullScreen = 0xE73F
    }

    /// <summary>
    /// Model of an application bar button showing a Segoe UI symbol.
    /// </summary>
    public sealed class SegoeGlyphBarButtonModel : BarButtonModel
    {
        private readonly SegoeGlyph _glyph;

        public SegoeGlyph Glyph { get { return _glyph; } }

        public SegoeGlyphBarButtonModel(SegoeGlyph glyph, ICommand command, string label, ItemAlignment alignment = ItemAlignment.Left)
            : base(command, label, alignment)
        {
            _glyph = glyph;
        }
    }
}
