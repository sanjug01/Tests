using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Navigation
{
    interface IBackCommandArgs
    {
        bool Handled { get; set; }
    }
}
