using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Helpers
{
    public class RdClientException : Exception
    {
        public RdClientException() : base() { }

        public RdClientException (string message) : base(message) { }

    }
}
