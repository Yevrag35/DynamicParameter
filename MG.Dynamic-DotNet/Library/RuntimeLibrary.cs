using MG.Dynamic.Parameter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.Dynamic.Library
{
    public class RuntimeLibrary : IRuntimeLibrary
    {
        private RuntimeDefinedParameterDictionary _paramDict;
        private bool disposed;

        #region PROPERTIES
        public int Count => this.InnerParameters.Count;
        protected List<IRuntimeParameter> InnerParameters { get; }
        public IEnumerable<string> Keys => this.InnerParameters.Select(x => x.GetKey(true));

        #endregion

        public RuntimeLibrary()
        {
            _paramDict = new RuntimeDefinedParameterDictionary();
            this.InnerParameters = new List<IRuntimeParameter>();
        }

        #region LIBRARY METHODS
        public bool Add(IRuntimeParameter parameter)
        {
            if (string.IsNullOrWhiteSpace(parameter.Name))
                throw new ArgumentException("Cannot add a parameter whose Name is not defined.");

            Predicate<IRuntimeParameter> exists = x => x.GetKey(true).Equals(parameter.GetKey(true));

            if (!this.InnerParameters.Exists(exists))
            {
                this.InnerParameters.Add(parameter);
                return true;
            }
            else
                return false;
        }
        public virtual RuntimeDefinedParameterDictionary AsParameterDictionary() => this.AsParameterDictionary(true);
        public RuntimeDefinedParameterDictionary AsParameterDictionary(bool useNameIfEmptyKey)
        {
            if (_paramDict.Count > 0)
                _paramDict.Clear();

            foreach (IRuntimeParameter parameter in this.InnerParameters)
            {
                string key = parameter.GetKey(useNameIfEmptyKey);

                if (string.IsNullOrWhiteSpace(key))
                    throw new InvalidKeyException(parameter);

                RuntimeDefinedParameter rtParam = parameter.AsRuntimeDefinedParameter();
                _paramDict.Add(key, rtParam);
            }
            return _paramDict;
        }
        public void Clear()
        {
            this.InnerParameters.Clear();
            _paramDict.Clear();
        }
        public bool Contains(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
                return false;

            return this.InnerParameters.Exists(x => parameterName.Equals(x.Name));
        }
        public bool Contains(IRuntimeParameter parameter) => this.InnerParameters.Contains(parameter);
        public bool ContainsKey(string key) => this.ContainsKey(key, true);
        public bool ContainsKey(string key, bool useNameIfEmpty)
        {
            if (string.IsNullOrWhiteSpace(key))
                return false;

            return this.InnerParameters.Exists(x => key.Equals(x.GetKey(useNameIfEmpty)));
        }
        public IEnumerator<IRuntimeParameter> GetEnumerator() => this.InnerParameters.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.InnerParameters.GetEnumerator();
        public IList<string> GetKeys(bool useNameIfEmptyKey = true) => this.InnerParameters.Select(x => x.GetKey(useNameIfEmptyKey)).ToList();
        public void Remove(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
                return;

            IRuntimeParameter param = this.InnerParameters.Find(x => x.Name.Equals(parameterName));
            if (param != null)
                this.InnerParameters.Remove(param);
        }

        #endregion

        //#region IDICTIONARY MEMBERS
        //RuntimeDefinedParameter IReadOnlyDictionary<string, RuntimeDefinedParameter>.this[string key] => _paramDict[key.ToLower()];
        //IEnumerable<RuntimeDefinedParameter> IReadOnlyDictionary<string, RuntimeDefinedParameter>.Values => _paramDict.Values;
        //IEnumerator<KeyValuePair<string, RuntimeDefinedParameter>> IEnumerable<KeyValuePair<string, RuntimeDefinedParameter>>.GetEnumerator()
        //{
        //    return _paramDict.GetEnumerator();
        //}
        //bool IReadOnlyDictionary<string, RuntimeDefinedParameter>.TryGetValue(string key, out RuntimeDefinedParameter value)
        //{
        //    value = null;
        //    if (_paramDict.TryGetValue(key.ToLower(), out RuntimeDefinedParameter outVal))
        //        value = outVal;

        //    return value != null;
        //}

        //#endregion

        public static explicit operator RuntimeDefinedParameterDictionary(RuntimeLibrary library) => library.AsParameterDictionary();
    }
}
