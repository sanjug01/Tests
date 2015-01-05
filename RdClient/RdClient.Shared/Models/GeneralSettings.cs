using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    [DataContract(IsReference = true)]
    public sealed class GeneralSettings : ModelBase
    {
        private bool _useThumbnails;

        public GeneralSettings()
        {
            this.SetDefaults();
        }

        [DataMember]
        public bool UseThumbnails
        {
            get
            {
                return _useThumbnails;
            }
            set
            {
                SetProperty(ref _useThumbnails, value);
            }
        }

        private void SetDefaults()
        {
            this.UseThumbnails = true;
        }
    }
}
