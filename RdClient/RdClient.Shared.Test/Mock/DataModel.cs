﻿using RdClient.Shared.Models;
using RdMock;

namespace RdClient.Shared.Test.Mock
{
    class DataModel : MockBase, IDataModel
    {
        public IDataStorage Storage { get; set; }

        public bool Loaded { get; set; }

        public void LoadFromStorage()
        {
            Invoke(new object[] { });
        }

        public ModelCollection<Desktop> Desktops { get; set; }

        public ModelCollection<Credentials> Credentials { get; set; }

        public ModelCollection<RdClient.Shared.Models.Thumbnail> Thumbnails { get; set; }
    }
}
