﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Models
{
    public interface IDataModel
    {
        IDataStorage Storage
        {
            set;
        }

        bool Loaded
        {
            get;
        }

        Task LoadFromStorage();

        ModelCollection<Desktop> Desktops
        {
            get;
        }

        ModelCollection<Credentials> Credentials
        {
            get;
        }
    }
}