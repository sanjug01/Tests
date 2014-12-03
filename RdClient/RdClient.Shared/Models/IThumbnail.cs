using System;
using Windows.UI.Xaml.Media;

namespace RdClient.Shared.Models
{
    interface IThumbnail
    {
        bool HasImage { get; }

        ImageSource Image { get; }

        void Update(uint width, uint height, byte[] imageBytes);
    }
}
