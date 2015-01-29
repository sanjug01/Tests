﻿namespace RdClient.Shared.Data
{
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public abstract class SerializableCompositionModel : IPersistentObject
    {
        private ICommand _save;

        protected SerializableCompositionModel()
        {
        }

        ICommand IPersistentObject.Save
        {
            get
            {
                Contract.Ensures(null != Contract.Result<ICommand>());
                if (null == _save)
                    _save = CreateSaveCommand();
                Contract.Assert(null != _save);
                return _save;
            }
        }

        protected abstract ICommand CreateSaveCommand();
    }
}
