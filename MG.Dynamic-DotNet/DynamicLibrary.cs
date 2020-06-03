using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic
{
    /// <summary>
    /// Represents a collection of <see cref="RuntimeDefinedParameter"/> or <see cref="IDynParam"/> classes that are keyed on
    /// the name of the parameter.  It also has the ability to match chosen ValidateSet values to the parameters' underlying 
    /// objects.
    /// </summary>
    public class DynamicLibrary : RuntimeDefinedParameterDictionary, IDisposable
    {
        #region FIELDS/CONSTANTS

        private bool disposed;
        private readonly List<IDynParam> _dynParams;
        private const BindingFlags NONPUB_INST = BindingFlags.Instance | BindingFlags.NonPublic;
        private const BindingFlags PUB_INST = BindingFlags.Public | BindingFlags.Instance;

        #endregion

        #region CONSTRUCTORS
        /// <summary>
        /// Initializes a blank instance of the dynamic library.
        /// </summary>
        public DynamicLibrary()
            : base() => _dynParams = new List<IDynParam>();

        #endregion

        #region METHODS

        #region ADDITIVE

        /// <summary>
        /// Adds the specified <see cref="IDynParam"/> interface to the library.
        /// </summary>
        /// <param name="dynamicParameter">The interface to add to the library.</param>
        /// <exception cref="ArgumentException">An item with the same key already exists within the library.</exception>
        /// <exception cref="ArgumentNullException">The interface is null</exception>
        public void Add(IDynParam dynamicParameter)
        {
            if (!base.ContainsKey(dynamicParameter.Name))
                base.Add(dynamicParameter.Name, dynamicParameter.AsRuntimeParameter());

            _dynParams.Add(dynamicParameter);
        }

        /// <summary>
        /// Adds the specified <see cref="RuntimeDefinedParameter"/> to the dictionary with the key the same as the parameter's name.
        /// </summary>
        /// <param name="rtParam">The parameter to add to the dictionary.</param>
        public void Add(RuntimeDefinedParameter rtParam) => base.Add(rtParam.Name, rtParam);

        /// <summary>
        /// Adds the interface elements of the specified collection to the end of the library.
        /// </summary>
        /// <param name="parameters">The collection whose elements should be added to the end of the library.
        /// The collection itself cannot be null, and cannot contain elements that are null.</param>
        /// <exception cref="ArgumentNullException"/>
        public void AddRange(IEnumerable<IDynParam> parameters)
        {
            foreach (IDynParam idyn in parameters)
            {
                this.Add(idyn);
            }
        }

        /// <summary>
        /// Adds the elements of the specified collection of <see cref="RuntimeDefinedParameter"/>'s to the end of the library.
        /// </summary>
        /// <param name="definedParameters">The collection whose elements should be added to the end of the library.
        /// The collection itself cannot be null, and cannot contain elements that are null.</param>
        /// <exception cref="ArgumentNullException"/>
        public void AddRange(IEnumerable<RuntimeDefinedParameter> definedParameters)
        {
            foreach (RuntimeDefinedParameter rtParam in definedParameters)
            {
                base.Add(rtParam.Name, rtParam);
            }
        }

        #endregion

        #region CLEAR
        /// <summary>
        /// Removes all keys, values, and <see cref="IDynParam"/> interfaces from the library.
        /// </summary>
        public new void Clear()
        {
            _dynParams.Clear();
            base.Clear();
        }

        #endregion

        #region CASTING
        private T Cast<T>(object o) => (T)o;
        //private IEnumerable<T> Cast<T>(IEnumerable ienum)
        //{
        //    return ienum.Cast<T>();
        //    //foreach (dynamic o in ienum)
        //    //{
        //    //    yield return this.Cast<T>(o);
        //    //}
        //}
        private T[] Cast<T>(object[] o)
        {
            var tArr = new T[o.Length];
            for (int i = 0; i < o.Length; i++)
            {
                tArr[i] = (T)o[i];
            }
            return tArr;
        }

        #endregion

        #region BACKING ITEMS

        /// <summary>
        /// Retrieves all of the underlying items from an <see cref="IDynParam"/> parameter and casts the results
        /// as a generic-type array.
        /// </summary>
        /// <typeparam name="T">The type to cast the results as.</typeparam>
        /// <param name="parameterName">The name of the parameter that implements <see cref="IDynParam"/> to retrieve its underlying values from.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidCastException"/>
        /// <exception cref="InvalidOperationException">Thrown when the library does not contain any parameters that inherit from <see cref="IDynParam"/>.</exception>
        public T[] GetBackingItems<T>(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");

            if (!this.LibraryContainsIDynParams())
                throw new LibraryContainsNoIDynsException();
    
            IDynParam idyp = _dynParams
                .Find(x => 
                    x.Name
                        .Equals(parameterName, StringComparison.CurrentCultureIgnoreCase));

            return idyp.GetBackingItems().Cast<T>().ToArray();
        }

        #endregion

        #region PARAMETER VALUES

        #region NON-GENERIC

        /// <summary>
        /// Retrieves the value chosen from the ValidateSet of the parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to retrieve the chosen value from.</param>
        /// <exception cref="ArgumentNullException"/>
        public object GetParameterValue(string parameterName)
        {
            object val = null;
            if (this.ParameterHasValue(parameterName))
            {
                val = base[parameterName].Value;
            }
            return val;
        }

        /// <summary>
        /// Retrieves the values that were chosen from the ValidateSet of the parameter.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to retrieve the chosen values from.</param>
        /// <exception cref="ArgumentNullException"/>
        public IEnumerable<object> GetParameterValues(string parameterName)
        {
            if (this.ParameterHasValue(parameterName) && base[parameterName].Value is IEnumerable ienum)
            {
                foreach (object o in ienum)
                {
                    yield return o;
                }
            }
        }

        #endregion

        #region GENERIC

        /// <summary>
        /// Retrieves the value chosen from the ValidateSet of the parameter and casts it as the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the result as.</typeparam>
        /// <param name="parameterName">The name of the parameter to retrieve the chosen value from.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidCastException"/>
        public T GetParameterValue<T>(string parameterName) where T : class
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

        /// <summary>
        /// Retrieves the values that were chosen from the ValidateSet of the parameter 
        /// as an array of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the results as.</typeparam>
        /// <param name="parameterName">The name of the parameter to retrieve the chosen values from.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidCastException"/>
        public IEnumerable<T> GetParameterValues<T>(string parameterName) where T : class
        {
            if (this.ParameterHasValue(parameterName))
            {
                IEnumerable<object> objArr = this.GetParameterValues(parameterName);
                foreach (object o in objArr)
                {
                    if (o is T tO)
                        yield return tO;

                    else
                        yield return this.Cast<T>(o);
                }
            }
        }

        #endregion

        #endregion

        #region UNDERLYING VALUES

        #region NON-GENERIC

        /// <summary>
        /// Retrieves the underlying value from an <see cref="IDynParam"/> parameter whose designated property value matches
        /// the chosen value from the ValidateSet.
        /// </summary>
        /// <param name="parameterName">The parameter that implements <see cref="IDynParam"/> to retrieve the chosen value from
        /// and match it to one of its underlying values.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="LibraryContainsNoIDynsException"/>
        public object GetUnderlyingValue(string parameterName)
        {
            object retVal = null;
            if (this.LibraryContainsIDynParams())
            {
                object oVal = this.GetParameterValue(parameterName);
                retVal = _dynParams.Single(
                    x => x.Name.Equals(parameterName)).GetItemFromChosenValue(oVal);
            }
            else
                throw new LibraryContainsNoIDynsException();

            return retVal;
        }

        /// <summary>
        /// Retrieves the underlying values from an <see cref="IDynParam"/> parameter whose designated property value matches
        /// any of the chosen values from the ValidateSet.
        /// </summary>
        /// <param name="parameterName">The parameter that implements <see cref="IDynParam"/> to retrieve the chosen values from
        /// and match it to any one of its underlying values.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        /// <exception cref="LibraryContainsNoIDynsException"/>
        public IEnumerable<object> GetUnderlyingValues(string parameterName)
        {
            if ( ! this.LibraryContainsIDynParams())
                return null;
            
            IEnumerable<object> oVals = this.GetParameterValues(parameterName);
            return _dynParams?
                .Single(x =>
                    x.Name.Equals(parameterName))
                        .GetItemsFromChosenValues(oVals);
        }

        #endregion

        #region GENERIC

        /// <summary>
        /// Retrieves the underlying value from an <see cref="IDynParam"/> parameter whose designated property value matches
        /// the chosen value from the ValidateSet.  It then casts the result to the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the result as.</typeparam>
        /// <param name="parameterName">The parameter that implements <see cref="IDynParam"/> to retrieve the chosen value from
        /// and match it to one of its underlying values.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidCastException"/>
        /// <exception cref="LibraryContainsNoIDynsException"/>
        public T GetUnderlyingValue<T>(string parameterName)
        {
            T tVal = default;
            if (this.LibraryContainsIDynParams() && _dynParams.OfType<DynamicParameter<T>>().Any())
            {
                DynamicParameter<T> casted = _dynParams
                    .OfType<DynamicParameter<T>>()
                        .Where(x => x.Name.Equals(parameterName))
                            .FirstOrDefault();

                if (casted != null)
                {
                    object oVal = this.GetParameterValue(parameterName);
                    tVal = casted.GetItemFromChosenValue(oVal);
                }
            }
            else
                throw new LibraryContainsNoIDynsException();

            return tVal;
        }

        /// <summary>
        /// Retrieves the underlying values from an <see cref="IDynParam"/> parameter whose designated property value matches
        /// any of the chosen values from the ValidateSet.  It then casts the results to a collection of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to cast the results as.</typeparam>
        /// <param name="parameterName">The parameter that implements <see cref="IDynParam"/> to retrieve the chosen values from
        /// and match it to any one of its underlying values.</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidCastException"/>
        /// <exception cref="LibraryContainsNoIDynsException"/>
        public IEnumerable<T> GetUnderlyingValues<T>(string parameterName)
        {
            if (this.LibraryContainsIDynParams() && _dynParams.OfType<DynamicParameter<T>>().Any())
            {
                IEnumerable<DynamicParameter<T>> casted = _dynParams
                    .OfType<DynamicParameter<T>>()
                        .Where(x => x.Name.Equals(parameterName));

                foreach (DynamicParameter<T> dp in casted)
                {
                    IEnumerable<object> oVal = this.GetParameterValues(parameterName);
                    foreach (T tVal in dp.GetItemFromChosenValues(oVal))
                    {
                        yield return tVal;
                    }
                }
            }
            else
                throw new LibraryContainsNoIDynsException();
        }

        #endregion

        #endregion

        #region CONDITIONAL

        /// <summary>
        /// Checks if the library contains any parameters who implement <see cref="IDynParam"/>.
        /// </summary>
        public bool LibraryContainsIDynParams() => _dynParams != null && _dynParams.Count > 0;

        /// <summary>
        /// Checks if the specified parameter's attributes contain the <see cref="ValidateSetAttribute"/>.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to check for the attribute.</param>
        /// <exception cref="ArgumentNullException"/>
        public bool ParameterIsValidateSet(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");

            bool result = false;
            if (base.ContainsKey(parameterName))
            {
                RuntimeDefinedParameter rtParam = base[parameterName];
                result = rtParam.Attributes.Any(x => x is ValidateSetAttribute);
            }
            return result;
        }

        /// <summary>
        /// Checks if the specified parameter has chosen value(s) from its ValidateSet.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to check for values.</param>
        /// <exception cref="ArgumentNullException"/>
        public bool ParameterHasValue(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentNullException("parameterName");

            bool result = false;
            if (base.ContainsKey(parameterName))
            {
                RuntimeDefinedParameter rtParam = base[parameterName];
                result = rtParam.IsSet && rtParam.Value != null;
            }
            return result;
        }

        #endregion

        #region IDISPOSABLE
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing && _dynParams.Count > 0)
            {
                foreach (IDynParam idp in _dynParams)
                {
                    Type[] genTypes = idp.GetType().GetGenericArguments();
                    if (genTypes == null)
                        continue;

                    for (int i = 0; i < genTypes.Length; i++)
                    {
                        Type gt = genTypes[i];
                        if (gt.GetInterfaces().Contains(typeof(IDisposable)))
                        {
                            foreach (IDisposable idis in Enumerable.Cast<IDisposable>(idp.GetBackingItems()))
                            {
                                idis.Dispose();
                            }
                        }
                    }
                }
            }

            disposed = true;
        }

        #endregion

        #endregion
    }
}
