using RdClient.Shared.ValidationRules;
using RdMock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Mock
{
    class ValidationRule<T> : MockBase, IValidationRule<T>
    {
        public IValidationResult Validate(T value)
        {
            return Invoke(new object[] { value }) as IValidationResult;
        }
    }
}
