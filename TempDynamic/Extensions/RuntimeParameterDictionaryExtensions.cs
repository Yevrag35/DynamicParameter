using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace TempDynamic.Extensions
{
    public static class RuntimeParameterDictionaryExtensions
    {
        public static RuntimeDefinedParameterDictionary Add(this RuntimeDefinedParameterDictionary dictionary, 
            RuntimeDefinedParameter parameter)
        {
            dictionary.Add(parameter.Name, parameter);
            return dictionary;
        }

        public static T GetValue<T>(this RuntimeDefinedParameterDictionary dictionary, string parameterName)
        {
            return dictionary.TryGetValue(parameterName, out RuntimeDefinedParameter rtParam)
                ? (T)rtParam.Value
                : default;
        }
        public static T[] GetValues<T>(this RuntimeDefinedParameterDictionary dictionary, string parameterName)
        {
            return GetEnumerableValues<T>(dictionary, parameterName).ToArray();
        }
        public static IEnumerable<T> GetEnumerableValues<T>(this RuntimeDefinedParameterDictionary dictionary, string parameterName)
        {
            return dictionary.TryGetValue(parameterName, out RuntimeDefinedParameter parameter) && parameter.Value is ICollection icol
                ? icol.Cast<T>()
                : (new T[0]);
        }
        public static IList<string> GetPossibles(this RuntimeDefinedParameterDictionary dictionary, string parameterName)
        {
            if (dictionary.TryGetValue(parameterName, out RuntimeDefinedParameter rtParam) &&
                rtParam.Attributes.FirstOrDefault(x => typeof(ValidateSetAttribute).Equals(x?.GetType())) is ValidateSetAttribute vSet)
            {
                return vSet.ValidValues;
            }
            else
            {
                return null;
            }
        }
    }
}
