namespace RdClient.Shared.Helpers
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Threading;

    public abstract class MutableObject : DisposableObject, INotifyPropertyChanged
    {
        private readonly ReaderWriterLockSlim _monitor;
        private PropertyChangedEventHandler _propertyChanged;

        protected MutableObject()
        {
            _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        protected IDisposable LockRead()
        {
            return ReadWriteMonitor.Read(_monitor);
        }

        protected IDisposable LockUpgradeableRead()
        {
            return ReadWriteMonitor.UpgradeableRead(_monitor);
        }

        protected IDisposable LockWrite()
        {
            return ReadWriteMonitor.Write(_monitor);
        }

        protected void EmitPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler;

            using (LockRead())
                handler = _propertyChanged;

            if (null != handler)
                handler(this, e);
        }

        protected void EmitPropertyChanged(string propertyName)
        {
            EmitPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            //Debug.WriteLine("propertyName: {0}, storage: {1}, value: {2}", propertyName, storage, value);

            if (object.Equals(storage, value))
            {
                return false;
            }
            else
            {
                storage = value;
                this.EmitPropertyChanged(propertyName);
                return true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                using (LockWrite()) _propertyChanged += value;
            }

            remove
            {
                using (LockWrite()) _propertyChanged -= value;
            }
        }
    }
}
