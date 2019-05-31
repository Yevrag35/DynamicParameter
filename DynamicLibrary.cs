using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic
{
    public class DynamicLibrary : RuntimeDefinedParameterDictionary
    {
        private readonly List<IDynParam> _dynParams;
        private const BindingFlags FLAGS = BindingFlags.Instance | BindingFlags.NonPublic;
        //private static readonly MethodInfo CAST_METHOD = typeof(DynamicLibrary).GetMethod("Cast", FLAGS);
        private const BindingFlags PUB_INST = BindingFlags.Public | BindingFlags.Instance;

        public DynamicLibrary()
            : base() => _dynParams = new List<IDynParam>();

        public void Add(IDynParam dynamicParameter)
        {
            if (!base.ContainsKey(dynamicParameter.Name))
                base.Add(dynamicParameter.Name, dynamicParameter.AsRuntimeParameter());

            _dynParams.Add(dynamicParameter);
        }

        private T Cast<T>(dynamic o) => (T)o;

        public object GetParameterValue(string parameterName)
        {
            object val = null;
            if (this.ParameterHasValue(parameterName))
            {
                val = base[parameterName].Value;
            }
            return val;
        }

        public T GetParameterValue<T>(string parameterName)
        {
            T tVal = default;
            if (this.ParameterHasValue(parameterName))
            {
                object val = base[parameterName].Value;
                tVal = val is T gah 
                    ? gah 
                    : this.Cast<T>(val);
            }
            return tVal;
        }

        public object GetUnderlyingValue(string parameterName)
        {
            object retVal = null;
            if (_dynParams.Count > 0)
            {
                object oVal = this.GetParameterValue(parameterName);
                retVal = _dynParams.Single(
                    x => x.Name.Equals(parameterName)).GetItemFromChosenValue(oVal);
            }
            return retVal;
        }
        public T GetUnderlyingValue<T>(string parameterName)
        {
            T tVal = default;
            object underVal = this.GetUnderlyingValue(parameterName);

            if (underVal != null)
                tVal = this.Cast<T>(underVal);
            
            return tVal;
        }

        public bool ParameterHasValue(string parameterName)
        {
            bool result = false;
            if (base.ContainsKey(parameterName))
            {
                RuntimeDefinedParameter rtParam = base[parameterName];
                result = rtParam.IsSet && rtParam.Value != null;
            }
            return result;
        }
    }
}
