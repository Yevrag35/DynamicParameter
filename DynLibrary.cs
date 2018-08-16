using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Dynamic
{
    public class Library : RuntimeDefinedParameterDictionary
    {
        public Library() { }

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
