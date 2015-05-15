namespace RdClient.Shared.Converters
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// Converter converts a collection of BarItemModel objects into another collection
    /// containing only models with specified alignment.
    /// If the Alignmment property hasn't been set, the converter returns the input collection.
    /// </summary>
    public sealed class BarItemsAlignmentFilteringConverter : MutableObject, IValueConverter
    {
        private bool _alignmentSet;
        private BarItemModel.ItemAlignment _alignment;
        static private TypeInfo _targetTypeInfo;

        static BarItemsAlignmentFilteringConverter()
        {
            _targetTypeInfo = typeof(IEnumerable<BarItemModel>).GetTypeInfo();
        }

        public BarItemsAlignmentFilteringConverter()
        {
            _alignmentSet = false;
            _alignment = BarItemModel.ItemAlignment.Left;
        }

        public BarItemModel.ItemAlignment Alignment
        {
            get { return _alignment; }
            set
            {
                this.SetProperty<BarItemModel.ItemAlignment>(ref _alignment, value);
                _alignmentSet = true;
            }
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            IEnumerable<BarItemModel> filteredItems = null;

            if (!targetType.GetTypeInfo().IsAssignableFrom(_targetTypeInfo))
                throw new ArgumentException("Requested target type is incompatible with IEnumerable<BarItemModel>", "targetType");

            if(null != value)
            {
                IEnumerable<BarItemModel> unfilteredItems = value as IEnumerable<BarItemModel>;

                if( null == unfilteredItems )
                    throw new ArgumentException("Input value is incompatible with IEnumerable<BarItemModel>", "value");

                if (!_alignmentSet)
                {
                    filteredItems = unfilteredItems;
                }
                else
                {
                    IList<BarItemModel> output = new List<BarItemModel>();

                    foreach (BarItemModel model in unfilteredItems)
                        if(model.Alignment == _alignment)
                            output.Add(model);
                    filteredItems = output;
                }
            }

            return filteredItems;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}
