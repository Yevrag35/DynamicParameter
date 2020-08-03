using MG.Dynamic.Parameter;
using MG.Dynamic.Parameter.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace MG.Dynamic.Library.Generic
{
    public class ComplexDynamicLibrary<TKey, TOutput> : IComplexDynamicLibrary<TKey, TOutput> where TKey : IConvertible
    {
        private bool disposed;
        private RuntimeDefinedParameterDictionary _paramDict;

        public int Count => this.DynamicParameters.Count;
        protected List<IComplexDynamicParameter<TKey, TOutput>> DynamicParameters { get; }
        public IReadOnlyList<IComplexDynamicParameter<TKey, TOutput>> Values => this.DynamicParameters;

        public IComplexDynamicParameter<TKey, TOutput> this[string name]
        {
            get => this.DynamicParameters.Find(x => x.Name.Equals(name?.ToLower()));
        }

        public ComplexDynamicLibrary()
        {
            this.DynamicParameters = new List<IComplexDynamicParameter<TKey, TOutput>>();
            _paramDict = new RuntimeDefinedParameterDictionary();
        }

        public RuntimeDefinedParameterDictionary AsParameterDictionary() => _paramDict;
        public RuntimeDefinedParameterDictionary AsParameterDictionary(bool useNameIfEmptyKey)
        {
            if (_paramDict.Count > 0)
                _paramDict.Clear();

            foreach (IComplexDynamicParameter<TKey, TOutput> parameter in this.DynamicParameters)
            {
                string key = parameter.GetKey(useNameIfEmptyKey);

                if (string.IsNullOrWhiteSpace(key))
                    throw new InvalidKeyException(parameter);

                RuntimeDefinedParameter rtParam = parameter.AsRuntimeDefinedParameter();
                _paramDict.Add(key, rtParam);
            }
            return _paramDict;
        }

        public void Dispose() => this.Dispose(true);
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (this.DynamicParameters.Count > 0)
                {
                    foreach (IDisposable disposable in this.DynamicParameters.SelectMany(x => x.GetBackingItems()).OfType<IDisposable>())
                    {
                        disposable.Dispose();
                    }
                }
                disposed = true;
            }
        }
        public IEnumerator<IComplexDynamicParameter<TKey, TOutput>> GetEnumerator() => this.DynamicParameters.GetEnumerator();
        IEnumerator<IRuntimeParameter> IEnumerable<IRuntimeParameter>.GetEnumerator()
        {
            foreach (IRuntimeParameter irp in this.DynamicParameters)
            {
                yield return irp;
            }
        }
        IEnumerator IEnumerable.GetEnumerator() => _paramDict.GetEnumerator();

        #region LIBRARY METHODS
        public bool Add(IComplexDynamicParameter<TKey, TOutput> parameter)
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
        public IList<TOutput> GetBackingItems(TKey key)
        {
            return this.DynamicParameters.FindAll(x => x.GetBackingItems().Any(bi => x.PropertyFunction(bi).Equals(key));
        }
        public IList<string> GetKeys(bool useNameIfEmptyKey) => this.DynamicParameters.Select(x => x.GetKey(useNameIfEmptyKey)).ToList();

        #endregion

        public static explicit operator RuntimeDefinedParameterDictionary(ComplexDynamicLibrary<TKey, TOutput> library) => library._paramDict;
    }
}
