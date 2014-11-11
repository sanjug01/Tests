namespace RdClient.Helpers
{
    using System.Collections.Generic;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Selector of data templates that looks up a template in the map using the name of the type of the input
    /// object as the key.
    /// The dictionary of type names and templates may be built in XAML.
    /// <code>
    /// <ns:TypeNameDataTemplateSelector>
    ///     <helpers:TypeNameDataTemplateSelector.Templates>
    ///         <DataTemplate x:Key="SegoeGlyphBarButtonModel">
    ///             <AppBarButton
    ///                 Icon="{Binding Glyph, Converter={StaticResource segoeToSymbolIcon}}"
    ///                 Label="{Binding Label}"
    ///                 Command="{Binding Command}"/>
    ///         </DataTemplate>
    ///         <DataTemplate x:Key="SeparatorBarItemModel">
    ///             <Rectangle Width="2" Height="Auto" Margin="0, 4, 0, 4" />
    ///         </DataTemplate>
    ///     </helpers:TypeNameDataTemplateSelector.Templates>
    /// </ns:TypeNameDataTemplateSelector>
    /// </code>
    /// </summary>
    public sealed class TypeNameDataTemplateSelector : DataTemplateSelector
    {
        private readonly Dictionary<string, DataTemplate> _templates;

        public TypeNameDataTemplateSelector()
        {
            _templates = new Dictionary<string, DataTemplate>();
        }

        public Dictionary<string, DataTemplate> Templates { get { return _templates; } }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            DataTemplate template;

            try
            {
                template = _templates[item.GetType().Name];
            }
            catch
            {
                template = base.SelectTemplateCore(item, container);
            }

            return template;
        }
    }
}
