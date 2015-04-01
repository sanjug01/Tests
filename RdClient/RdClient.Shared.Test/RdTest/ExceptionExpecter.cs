using System;

namespace RdClient.Shared.Test.UAP
{
    public class ExceptionExpecter
    {
        public static bool ExpectException<T>(Action action) where T : System.Exception
        {
            bool caught = false;

            try
            {
                action();
            }
            catch(T /* e */)
            {
                caught = true;
            }

            return caught;
        }
    }
}
