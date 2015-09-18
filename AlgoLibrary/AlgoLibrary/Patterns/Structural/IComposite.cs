using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    public interface ICompositeBase
    {
        int Operation(string param);
    }
    public interface IComposite : ICompositeBase
    {
        bool Add(ICompositeBase node);
        bool Remove(ICompositeBase node);

        // optional iterator
        ICompositeBase GetChild(int n);
    }

    // a Leaf may have no extra methods that the base
    public interface ICompositeLeaf : ICompositeBase
    {
    }
}
