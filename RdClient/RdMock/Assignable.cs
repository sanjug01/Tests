using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
namespace RdMock
{
    public class Assignable
    {
        private static bool TypeAssignable(Type to, Type from)
        {
            IEnumerable<Type> toInterfaces = to.GetTypeInfo().ImplementedInterfaces;
            IEnumerable<Type> fromInterfaces = from.GetTypeInfo().ImplementedInterfaces;

            if (to == from)
            {
                return true;
            }
            else if (from.IsSubclassOf(to))
            {
                return true;
            }
            else if (to.IsInterface)
            {
                if(fromInterfaces.Contains(to))
                {
                    return true;
                }

                foreach (Type fromInterface in fromInterfaces)
                {
                    if (fromInterface.IsSubclassOf(to))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsAssignable(Type to, Type from)
        {
            bool result = TypeAssignable(to, from);

            if(result == false && from.IsGenericParameter)
            {
                foreach (Type constraint in from.GetGenericParameterConstraints())
                {
                    if(TypeAssignable(to, constraint))
                    {
                        return true;
                    }
                }
            }

            return result;
        }
    }
}
