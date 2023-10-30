using MG.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;

namespace TempDynamic
{
    public interface IPowerShellDynamicParameter
    {
        RuntimeDefinedParameter AsRuntimeParameter();
        object GetChosenValue();
    }

    public interface IPowerShellDynamicParameter<T> : IPowerShellDynamicParameter
    {
        new T GetChosenValue();
        T[] GetChosenValues();
    }
}
