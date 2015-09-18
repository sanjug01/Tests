using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Structural
{
    // alternative should be a class with a protected overridalbe method
    public interface IAdaptee
    {
        int SpecificRequest(object o);
    }

    public abstract class AdapteeBase
    {
        protected abstract int SpecificRequest(object o);
    }
}
