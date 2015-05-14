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
        private static TypeInfo _targetTypeInfo;

        static SegoeGlyphToSymbolIconConverter()
        {
            _glyphTranslation = new SortedDictionary<SegoeGlyph, Symbol>()
            {
                { SegoeGlyph.Delete, Symbol.Delete },
                { SegoeGlyph.Home, Symbol.Home },
                { SegoeGlyph.Forward, Symbol.Forward },
                { SegoeGlyph.Back, Symbol.Back },
                { SegoeGlyph.Settings, Symbol.Setting },
                { SegoeGlyph.Search, Symbol.Find },
                { SegoeGlyph.People, Symbol.People },
                { SegoeGlyph.Add, Symbol.Add },
                { SegoeGlyph.Edit, Symbol.Edit },
            };
            //
            // Getting TypeInfo for a type is not free, so we do it once and use the cached value
            // for conversion parameter verification.
            //
            _targetTypeInfo = typeof(SymbolIcon).GetTypeInfo();
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Contract.Ensures(null != Contract.Result<object>());

            if(!(value is SegoeGlyph))
                throw new ArgumentException("Invalid value type", "value");
            if (!targetType.GetTypeInfo().IsAssignableFrom(_targetTypeInfo))
                throw new ArgumentException(string.Format("Invalid target type {0}.{1}", targetType.Namespace, targetType.Namespace), "targetType");

            return new SymbolIcon(_glyphTranslation[(SegoeGlyph)value]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Contract.Ensures(null != Contract.Result<object>());
            //
            // Reverse translation is slow - it scans through the translation dictionary.
            // The converter is not designed to perform reverse conversions, but the data is available,
            // so it does.
            //
            if (!(value is SymbolIcon))
                throw new ArgumentException("Invalid value", "value");
            if (!targetType.Equals(typeof(SegoeGlyph)))
                throw new ArgumentException("Invalid target type", "targetType");

            foreach (var pair in _glyphTranslation)
                if (pair.Value == ((SymbolIcon)value).Symbol)
                    return pair.Key;

            throw new KeyNotFoundException();
        }
    }
}
