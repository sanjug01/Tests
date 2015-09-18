using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoLibrary.Patterns.Creational
{


    // big subsystem, with interconnected components
    public interface ISubSystem
    {
        int IntOper1();
        int IntOper2();
        string StringOper1();
        string StringOper2();
        string StringOper3();
    }

    // simplified interface
    public interface IFacade
    {
        int IntOper();
        string StringOper();
    }

    public class FacadeBase : IFacade
    {
        private readonly ISubSystem _subSystem;

        // the facade may still allow access to the subsystem
        public ISubSystem SubSystem { get { return _subSystem; } }

        public FacadeBase(ISubSystem subSystem)
        {
            _subSystem = subSystem;
        }

        public int IntOper()
        {
            return _subSystem.IntOper1();
        }

        public string StringOper()
        {
            return _subSystem.StringOper2();
        }
    }
}
