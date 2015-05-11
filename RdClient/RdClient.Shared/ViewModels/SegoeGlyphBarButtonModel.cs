namespace RdClient.Shared.ViewModels
{
    using System.Windows.Input;

    /// <summary>
    /// Subset of glyphs for use with the Segoe MDL2 Assets font.
    /// http://msdn.microsoft.com/en-us/library/windows/apps/jj841126.aspx
    /// </summary>
    public enum SegoeGlyph
    {
        Help = 0x003F,
        Trash = 0xE107,
        Home = 0xE10F,
        Forward = 0xe111,
        Back = 0xE112,
        Settings = 0xE115,
        Find = 0xE11A,
        People = 0xE125,
        Add = 0xE109,
        More = 0xE10C,
        Edit = 0xE104,
        ZoomIn = 0xE1A3,
        ZoomOut = 0xE1A4,
        MultiSelection = 0xE762,
        Keyboard =0xE765,
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
