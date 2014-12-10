using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Navigation.Extensions
{
    public interface IExecutionDeferrer
    {
        bool TryDeferToUI(Action action);

        void DeferToUI(Action action);
    }
}
