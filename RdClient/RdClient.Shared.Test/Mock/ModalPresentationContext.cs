using RdClient.Shared.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    class ModalPresentationContext : IModalPresentationContext
    {
        public object Result { get; private set; }
        void IModalPresentationContext.Dismiss(object result)
        {
            this.Result = result;
        }      
    }
}
