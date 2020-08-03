using MG.Dynamic.Parameter;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace MG.Dynamic.Library
{
    public interface IDynamicLibrary : IDisposable, IRuntimeLibrary, IEnumerable<IDynParam>
    {

    }
}
