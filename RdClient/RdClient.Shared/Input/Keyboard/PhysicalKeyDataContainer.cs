namespace RdClient.Shared.Input.Keyboard
{
    using System;

    public sealed class PhysicalKeyDataContainer
    {
        private IPhysicalKeyData _keyData;

        public PhysicalKeyDataContainer()
        {
        }

        public IPhysicalKeyData KeyData
        {
            get { return _keyData; }
            set { _keyData = value; }
        }

        /// <summary>
        /// Cast the key data object to the specified type and call the parameter action delegate with the cast object
        /// if it is not null.
        /// </summary>
        /// <typeparam name="T">Type to that the KeyData property is cast</typeparam>
        /// <param name="action">Action delegate that is called if the key data object is of the generic parameter type</param>
        /// <returns>The container object so DoIf<> calls can be chained together.</returns>
        public PhysicalKeyDataContainer DoIf<T>(Action<T> action) where T : class
        {
            T param = _keyData as T;

            if (null != param)
                action(param);

            return this;
        }
    }
}
