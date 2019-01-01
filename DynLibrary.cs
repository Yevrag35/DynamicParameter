using System;
using System.Management.Automation;

namespace MG.Dynamic
{
    public class ParameterLibrary : RuntimeDefinedParameterDictionary
    {
        public ParameterLibrary() { }

        public void AddParameter(Parameter p)
        {
            if (!ContainsValue(p))
            {
                Add(p.Name, p);
            }
        }
        public void RemoveParameter(Parameter p)
        {
            if (ContainsValue(p))
            {
                Remove(p.Name);
            }
        }
    }
}
