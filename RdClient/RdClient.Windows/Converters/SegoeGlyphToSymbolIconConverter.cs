namespace RdClient.Converters
{
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Reflection;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Converter of SegoeGlyph enumeration defined in the shared portable library to SymbolIcon.
    /// </summary>
    /// <remarks>The shared portable library has to target desktop .Net that doesn't include the Symbol
    /// enum and SymbolIcon class, so the library defines its own representation of Segoe UI app bar buttons.
    /// The converter is used to convert values from the portable library to app bar button icons in XAML bindings.</remarks>
    public sealed class SegoeGlyphToSymbolIconConverter : IValueConverter
    {
        private static readonly IDictionary<SegoeGlyph, Symbol> _glyphTranslation;

        static SegoeGlyphToSymbolIconConverter()
        {
            _glyphTranslation = new SortedDictionary<SegoeGlyph, Symbol>()
            {
                { SegoeGlyph.Trash, Symbol.Delete },
                { SegoeGlyph.Forward, Symbol.Forward },
                { SegoeGlyph.Back, Symbol.Back },
                { SegoeGlyph.Settings, Symbol.Setting },
                { SegoeGlyph.Find, Symbol.Find },
                { SegoeGlyph.People, Symbol.People }
            };
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Ensures(null != Contract.Result<object>());

            if(!(value is SegoeGlyph) || !targetType.GetTypeInfo().IsAssignableFrom(typeof(SymbolIcon).GetTypeInfo()))
                throw new NotImplementedException();

            SegoeGlyph glyph = (SegoeGlyph)value;
            Symbol symbol = _glyphTranslation[glyph];
            return new SymbolIcon(symbol);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Contract.Ensures(null != Contract.Result<object>());

            //
            // Reverse translation is slow - it scans through the translation dictionary.
            //
            if(!(value is SymbolIcon) || !targetType.Equals(typeof(SegoeGlyph)))
                throw new NotImplementedException();

            foreach (var pair in _glyphTranslation)
                if (pair.Value == ((SymbolIcon)value).Symbol)
                    return pair.Key;

            throw new KeyNotFoundException();
        }
    }
}
