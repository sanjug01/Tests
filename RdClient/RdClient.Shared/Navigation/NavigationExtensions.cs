namespace RdClient.Shared.Navigation
{
    using System;
    using System.Diagnostics.Contracts;

    public static class NavigationExtensions
    {
        /// <summary>
        /// Extension that tests if an object can be cast to a class, and if it can, calls
        /// an action delegate passing the cast object as a parameter.
        /// </summary>
        /// <typeparam name="IExt">Class or interface to which the input object is cast.</typeparam>
        /// <param name="obj">Object for that the extension is called as a method.</param>
        /// <param name="action">Delegate that takes one parameter of the type specified by IExt.</param>
        /// <remarks>The extension guarantees that the action delegate is called with a non-null parameter.</remarks>
        public static void CastAndCall<IExt>( this object obj, Action<IExt> action ) where IExt : class
        {
            Contract.Requires(null != obj);
            Contract.Requires(null != action);

            IExt i = obj as IExt;

            if( null != i )
            {
                action(i);
            }
        }
    }
}
