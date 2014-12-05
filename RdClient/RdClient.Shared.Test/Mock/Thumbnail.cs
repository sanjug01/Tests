using RdClient.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RdMock;
using RdClient.Shared.CxWrappers;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace RdClient.Shared.Test.Mock
{
    public class Thumbnail : MockBase, IThumbnail
    {
        public bool HasImage { get; set; }

        public BitmapImage Image { get; set; }

        public Task Update(IRdpScreenSnapshot snapshot)
        {
            return Task.FromResult(Invoke(new object[]{snapshot}));
        }       
    }
}
