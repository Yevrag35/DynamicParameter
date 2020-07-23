using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace MG.Dynamic.Library
{
    public interface IRuntimeLibrary
    {
        RuntimeDefinedParameterDictionary AsParameterDictionary();
    }
}
