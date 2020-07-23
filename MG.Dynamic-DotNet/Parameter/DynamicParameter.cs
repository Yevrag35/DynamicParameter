using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic.Parameter
{
    /// <summary>
    /// A class to be used when constructing a <see cref="DynamicLibrary"/> or <see cref="RuntimeDefinedParameterDictionary"/>
    /// without the need for a collection of attributes.  This class can store underlying values that create a dynamic 
    /// 'ValidateSet' off of one of its properties.  The generic type is of the underlying items type.
    /// </summary>
    /// <typeparam name="T">The type of the underlying items for the ValidateSet.</typeparam>
    public class DynamicParameter<T> : RuntimeParameter, IDynParam<T>, IDynParam
    {
        #region FIELDS/CONSTANTS
        private const BindingFlags PUB_INST = BindingFlags.Public | BindingFlags.Instance;

        private Func<T, IConvertible> _propertyFunc;

        //private List<string> _aliases;
        //private List<string> _items;
        private HashSet<string> _items;
        //private List<T> _backingItems;
        private string _mappedProperty;

        #endregion

        #region PROPERTIES

        public IList<T> BackingItems { get; protected set; } = new List<T>();

        /// <summary>
        /// The underlying type of the backend item collection that signifies this class's generic constraint.
        /// </summary>
        public Type BackingItemType => typeof(T);

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a blank instance of a DynamicParameter.
        /// </summary>
        public DynamicParameter()
        {
            //_aliases = new List<string>();
            _items = new HashSet<string>();
            //_backingItems = new List<T>();
        }

        /// <summary>
        /// Initializes a blank instance of a DynamicParameter with the specified name.
        /// </summary>
        /// <param name="name">The name of the dynamic parameter.</param>
        public DynamicParameter(string name)
            : this() => this.Name = name;

        /// <summary>
        /// Initializes a blank instance of a DynamicParameter with the specified name, along with
        /// specifying the property type of the future ValidateSet.
        /// </summary>
        /// <param name="name">The name of the dynamic parameter.</param>
        /// <param name="parameterType">The property type of the ValidateSet.</param>
        public DynamicParameter(string name, Type parameterType)
            : this(name) => this.ParameterType = parameterType;

        /// <summary>
        /// Initializes a new instance of a DynamicParameter with the specified name.  A generic <see cref="IEnumerable{T}"/>
        /// collection is used for the ValidateSet with the specifying parameter type.
        /// </summary>
        /// <param name="name">The name of the dynamic parameter.</param>
        /// <param name="parameterType">The property type for the ValidateSet.</param>
        /// <param name="items">The generic <see cref="IEnumerable"/> to use in the ValidateSet.</param>
        public DynamicParameter(string name, Type parameterType, IEnumerable<T> items)
        {
            this.Name = name;
            this.ParameterType = parameterType;
            //_aliases = new List<string>();
            this.BackingItems = new List<T>(items);
            _items = new HashSet<string>();
        }

        public DynamicParameter(string name, bool parameterTypeIsArray, IEnumerable<T> items, Expression<Func<T, IConvertible>> propertyExpression,
            params string[] aliases)
        {
            this.Name = name;
            if (propertyExpression.Body is MemberExpression memEx)
                _mappedProperty = memEx.Member.Name;

            else if (propertyExpression.Body is UnaryExpression unEx && unEx.Operand is MemberExpression unExMem)
                _mappedProperty = unExMem.Member.Name;

            else
                throw new ArgumentException("propertyExpression is not a valid member expression");

            if (aliases != null)
                this.Aliases = new List<string>(aliases);

            _propertyFunc = propertyExpression.Compile();
            this.BackingItems = new List<T>(items);
            var convertibles = new List<IConvertible>(items.Select(_propertyFunc));
            _items = new HashSet<string>();
            convertibles.ForEach((ic) =>
            {
                _items.Add(Convert.ToString(ic));
            });

            Type t = typeof(string);
            if (parameterTypeIsArray)
                t = t.MakeArrayType();

            this.ParameterType = t;
        }

        /// <summary>
        /// Initializes a new instance of a DynamicParameter with the specified name.  
        /// A generic <see cref="IEnumerable{T}"/> collection is used to build the ValidateSet along
        /// with an accompanying function to define the <see cref="string"/> property to use.
        /// </summary>
        /// <param name="name">The name of the dynamic parameter.</param>
        /// <param name="items">The generic <see cref="IEnumerable"/> containing the underlying values for the parameter.</param>
        /// <param name="validateSetProperty">The function predicate matching to the generic type's property that is of the type <see cref="string"/>,
        /// which will be used to generate the ValidateSet.</param>
        /// <param name="mappingProperty">The name of the <see cref="IEnumerable"/> type's property specified in the preceeding function.</param>
        /// <param name="parameterTypeIsArray">Indicates whether the ValidateSet should accept more than value.</param>
        [Obsolete]
        public DynamicParameter(string name, IEnumerable<T> items, Func<T, string> validateSetProperty, string mappingProperty, bool parameterTypeIsArray = false)
        {
            this.Name = name;
            _mappedProperty = mappingProperty;
            Type t = typeof(string);
            //_aliases = new List<string>();
            this.BackingItems = new List<T>(items);
            _items = new HashSet<string>(items.Select(validateSetProperty));

            if (parameterTypeIsArray)
                t = t.MakeArrayType();

            this.ParameterType = t;
        }

        /// <summary>
        /// Initializes a new instance of a DynamicParameter with the specified name.  
        /// A generic <see cref="IEnumerable{T}"/> collection is used to build the ValidateSet along
        /// with an accompanying function to define the <see cref="ValueType"/> property to use.
        /// </summary>
        /// <param name="name">The name of the dynamic parameter.</param>
        /// <param name="items">The generic <see cref="IEnumerable"/> containing the underlying values for the parameter.</param>
        /// <param name="validateSetProperty">The function predicate matching to the generic type's property that is of the type <see cref="ValueType"/>,
        /// which will be used to generate the ValidateSet.</param>
        /// <param name="mappingProperty">The name of the <see cref="IEnumerable"/> type's property specified in the preceeding function.</param>
        /// <param name="parameterTypeIsArray">Indicates whether the ValidateSet should accept more than value.</param>
        [Obsolete]
        public DynamicParameter(string name, IEnumerable<T> items, Func<T, ValueType> validateSetProperty, string mappingProperty, bool parameterTypeIsArray = false)
        {
            this.Name = name;
            _mappedProperty = mappingProperty;
            //_aliases = new List<string>();
            this.BackingItems = new List<T>(items);
            _items = new HashSet<string>();
            Type t = null;
            foreach (ValueType vt in items.Select(validateSetProperty))
            {
                _items.Add(Convert.ToString(vt));
                if (t == null)
                    t = vt.GetType();
            }
            if (parameterTypeIsArray)
                t = t.MakeArrayType();

            this.ParameterType = t;
        }

        #endregion

        #region METHODS

        /// <summary>
        /// Retrieves all the underlying objects that were used to build the ValidateSet.
        /// </summary>
        object[] IDynParam.GetBackingItems()
        {
            var objArr = new object[this.BackingItems.Count];
            for (int i = 0; i < this.BackingItems.Count; i++)
            {
                objArr[i] = this.BackingItems[i];
            }
            return objArr;
        }
        public T[] GetBackingItems()
        {
            var tArr = new T[this.BackingItems.Count];
            this.BackingItems.CopyTo(tArr, 0);
            return tArr;
        }

        /// <summary>
        /// Finds the underlying object that matches the designated property used to build a ValidateSet attribute.
        /// </summary>
        /// <param name="chosenValue">The value selected after IDynamicParameters has been processed.</param>
        object IDynParam.GetItemFromChosenValue(object chosenValue)
        {
            T outVal = default;
            for (int i = 0; i < this.BackingItems.Count; i++)
            {
                T bi = this.BackingItems[i];
                IConvertible biVal = _propertyFunc(bi);
                if (Convert.ToString(biVal).Equals(chosenValue))
                {
                    outVal = bi;
                    break;
                }
            }
            return outVal;
        }

        /// <summary>
        /// Finds the underlying objects that match the designated property used to build a ValidateSet attribute.
        /// </summary>
        /// <param name="chosenValues">The values selected after IDynamicParameters has been processed.</param>
        IEnumerable<object> IDynParam.GetItemsFromChosenValues(IEnumerable<object> chosenValues)
        {
            if (!string.IsNullOrEmpty(_mappedProperty))
            {
                PropertyInfo pi = typeof(T).GetProperty(_mappedProperty, PUB_INST);
                if (pi != null)
                {
                    foreach (T bi in this.BackingItems)
                    {
                        foreach (object val in chosenValues)
                        {
                            if (pi.GetValue(bi).Equals(val))
                            {
                                yield return bi;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Finds the underlying object that matches the designated property used to build a ValidateSet attribute and
        /// casts the result as the class's generic type.
        /// </summary>
        /// <param name="chosenValue">The value selected after IDynamicParameters has been processed.</param>
        public T GetItemFromChosenValue(object chosenValue)
        {
            T retVal = default;
            for (int i = 0; i < this.BackingItems.Count; i++)
            {
                T bi = this.BackingItems[i];
                if (Convert.ToString(_propertyFunc(bi)).Equals(chosenValue))
                {
                    retVal = bi;
                    break;
                }
            }
            return retVal;
        }

        public IEnumerable<T> GetItemsFromChosenValues(IEnumerable<object> chosenValues)
        {
            return this.BackingItems.Where(x => chosenValues.Any(o => Convert.ToString(_propertyFunc(x)).Equals(o)));
        }

        #endregion
    }
}
