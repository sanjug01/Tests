﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Helpers
{
    public interface ILocalizedStrings
    {
        string GetLocalizedString(string key);
    }
}