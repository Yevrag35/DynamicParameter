using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.Dynamic
{
    public class ParameterLibrary : RuntimeDefinedParameterDictionary
    {
        public ParameterLibrary() : base() { }

        public ParameterLibrary(IEnumerable<ParameterDefiner> ps)
            : base() => this.Add(ps);

        public void Add(ParameterDefiner p)
        {
            if (!base.ContainsValue(p))
                base.Add(p.Name, p);
            
        }
        public void Add(IEnumerable<ParameterDefiner> ps)
        {
            var pArr = ps.ToArray();
            for (int i = 0; i < pArr.Length; i++)
            {
                var p = pArr[i];
                this.Add(p);
            }
        }

        public void RemoveParameter(ParameterDefiner p)
        {
            if (base.ContainsValue(p))
                base.Remove(p.Name);
        }

        public object GetDefinedValue(string pName)
        {
            if (base.ContainsKey(pName))
                return this[pName].Value;
            
            else
                throw new ArgumentException("The key '" + pName + "' is not present in this library!");
        }
        public T GetDefinedValue<T>(string pName)
        {
            dynamic val = this.GetDefinedValue(pName);
            return (T)val;
        }
    }
}
