namespace RdClient.Shared.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IPresentationCompletion
    {
        event EventHandler<PresentationCompletionEventArgs> Completed;
        void EmitCompleted(IPresentableView view, object result);
    }
}
