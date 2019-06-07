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
        public void AddRange(IEnumerable<IDynParam> parameters)
        {
            foreach (IDynParam idyn in parameters)
            {
                this.Add(idyn);
            }
        }


        private T Cast<T>(dynamic o) => (T)o;
        private IEnumerable<T> Cast<T>(IEnumerable ienum)
        {
            var tList = new List<T>();
            foreach (dynamic o in ienum)
            {
                tList.Add(this.Cast<T>(o));
            }
            return tList;
        }

        public object GetParameterValue(string parameterName)
        {
            object val = null;
            if (this.ParameterHasValue(parameterName))
            {
                val = base[parameterName].Value;
            }
            return val;
        }
        public object[] GetParameterValues(string parameterName)
        {
            object[] vals = null;
            if (this.ParameterHasValue(parameterName))
            {
                vals = base[parameterName].Value as object[];
            }
            return vals;
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
        public T[] GetParameterValues<T>(string parameterName)
        {
            T[] tArr = null;
            if (this.ParameterHasValue(parameterName))
            {
                object[] objArr = this.GetParameterValues(parameterName);
                tArr = new T[objArr.Length];
                for (int i = 0; i < objArr.Length; i++)
                {
                    if (objArr[i] is T tObj)
                        tArr[i] = tObj;
                }
            }
            return tArr;
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
        public IEnumerable<object> GetUnderlyingValues(string parameterName)
        {
            var list = new List<object>();
            if (_dynParams.Count > 0)
            {
                object[] oVals = this.GetParameterValues(parameterName);
                list.AddRange(_dynParams.Single(
                    x => x.Name.Equals(parameterName)).GetItemsFromChosenValues(oVals));
            }
            return list;
        }

        public T GetUnderlyingValue<T>(string parameterName)
        {
            T tVal = default;
            object underVal = this.GetUnderlyingValue(parameterName);

            if (underVal != null)
                tVal = this.Cast<T>(underVal);
            
            return tVal;
        }

        public IEnumerable<T> GetUnderlyingValues<T>(string parameterName)
        {
            IEnumerable<object> underVals = this.GetUnderlyingValues(parameterName);

            if (underVals != null)
            {
                IEnumerable<T> tArr = this.Cast<T>(underVals);
                return tArr;
            }
            else
                return null;
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
