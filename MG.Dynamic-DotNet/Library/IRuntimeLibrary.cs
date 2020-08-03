using MG.Dynamic.Parameter;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace MG.Dynamic.Library
{
    public interface IRuntimeLibrary : IEnumerable<IRuntimeParameter>
    {
        int Count { get; }
        RuntimeDefinedParameterDictionary AsParameterDictionary();
        RuntimeDefinedParameterDictionary AsParameterDictionary(bool useNameIfEmptyKey);
        IList<string> GetKeys(bool useNameIfEmptyKey);
    }
}
