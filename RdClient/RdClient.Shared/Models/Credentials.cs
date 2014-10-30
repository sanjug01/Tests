using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    public struct Credentials
    {
        public string username;
        public string domain;
        public string password;
        public bool haveBeenPersisted;
    }
}
