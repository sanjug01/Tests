namespace RdClient.Shared.Converters
{
    using System;
    using System.Reflection;
    using System.Runtime.InteropServices.WindowsRuntime;
    using Windows.Storage.Streams;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media.Imaging;

    public sealed class BytesToBitmapConverter : IValueConverter
    {
        private static readonly TypeInfo _bitmapTypeInfo = typeof(BitmapImage).GetTypeInfo();

        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (null != targetType && !targetType.GetTypeInfo().IsAssignableFrom(_bitmapTypeInfo))
                throw new ArgumentException("Target type must be a bitmap image", "targetType");

            BitmapImage image = null;

            if(null != value)
            {
                if (!(value is byte[]))
                    throw new ArgumentException("Value must be a byte array");

                byte[] imageBytes = (byte[])value;
                IRandomAccessStream stream = new InMemoryRandomAccessStream();

                stream.WriteAsync(imageBytes.AsBuffer()).AsTask<uint, uint>().Wait();
                stream.Seek(0);
                image = new BitmapImage();
                image.SetSourceAsync(stream).AsTask();
            }

            return image;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new System.NotImplementedException();
        }
    }
}
