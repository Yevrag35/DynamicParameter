using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic.Parameter.Generic
{
    /// <summary>
    /// A class to be used when constructing a <see cref="DynamicLibrary"/> or <see cref="RuntimeDefinedParameterDictionary"/>
    /// without the need for a collection of attributes.  This class can store underlying values that create a dynamic 
    /// 'ValidateSet' off of one of its properties.  The generic type is of the underlying items type.
    /// </summary>
    /// <typeparam name="TOutput">The type of the underlying items for the ValidateSet.</typeparam>
    public class DynamicParameter<TOutput> : RuntimeParameter, IDynParam<TOutput>, IDynParam
    {
        #region FIELDS/CONSTANTS
        private const BindingFlags PUB_INST = BindingFlags.Public | BindingFlags.Instance;

        private Func<TOutput, IConvertible> _propertyFunc;

        //private List<string> _aliases;
        //private List<string> _items;
        private HashSet<string> _items;
        private protected List<TOutput> _backingItems;
        private string _mappedProperty;

        #endregion

        #region PROPERTIES

        public IList<TOutput> BackingItems => _backingItems;

        /// <summary>
        /// The underlying type of the backend item collection that signifies this class's generic constraint.
        /// </summary>
        public Type BackingItemType => typeof(TOutput);

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a blank instance of a DynamicParameter.
        /// </summary>
        //public DynamicParameter()
        //{
        //    //_aliases = new List<string>();
        //    _items = new HashSet<string>();
        //    //_backingItems = new List<TOutput>();
        //}

        /// <summary>
        /// Initializes a blank instance of a DynamicParameter with the specified name.
        /// </summary>
        /// <param name="name">The name of the dynamic parameter.</param>
        public DynamicParameter(string name) : base(name)
        {
            _backingItems = new List<TOutput>();
            _items = new HashSet<string>();
            this.ParameterType = typeof(string);
        }

        /// <summary>
        /// Initializes a blank instance of a DynamicParameter with the specified name, along with
        /// specifying the property type of the future ValidateSet.
        /// </summary>
        /// <param name="name">The name of the dynamic parameter.</param>
        /// <param name="parameterType">The property type of the ValidateSet.</param>
        public DynamicParameter(string name, Type parameterType)
            : this(name) => this.ParameterType = parameterType;

        /// <summary>
        /// Initializes a new instance of a DynamicParameter with the specified name.  A generic <see cref="IEnumerable{TOutput}"/>
        /// collection is used for the ValidateSet with the specifying parameter type.
        /// </summary>
        /// <param name="name">The name of the dynamic parameter.</param>
        /// <param name="parameterType">The property type for the ValidateSet.</param>
        /// <param name="items">The generic <see cref="IEnumerable"/> to use in the ValidateSet.</param>
        public DynamicParameter(string name, Type parameterType, IEnumerable<TOutput> items)
            : base(name)
        {
            this.ParameterType = parameterType;
            //_aliases = new List<string>();
            _backingItems = new List<TOutput>(items);
            _items = new HashSet<string>();
        }

        public DynamicParameter(string name, bool parameterTypeIsArray, IEnumerable<TOutput> items, Expression<Func<TOutput, IConvertible>> propertyExpression,
            params string[] aliases)
            : base(name)
        {
            if (propertyExpression.Body is MemberExpression memEx)
                _mappedProperty = memEx.Member.Name;

            else if (propertyExpression.Body is UnaryExpression unEx && unEx.Operand is MemberExpression unExMem)
                _mappedProperty = unExMem.Member.Name;

            else
                throw new ArgumentException("propertyExpression is not a valid member expression");

            if (aliases != null)
                this.Aliases = new List<string>(aliases);

            _propertyFunc = propertyExpression.Compile();
            _backingItems = new List<TOutput>(items);
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

        #endregion

        #region METHODS

        /// <summary>
        /// Retrieves all the underlying objects that were used to build the ValidateSet.
        /// </summary>
        ICollection IDynParam.GetBackingItems() => _backingItems;
        //{
            //var objArr = new object[this.BackingItems.Count];
            //for (int i = 0; i < this.BackingItems.Count; i++)
            //{
            //    objArr[i] = this.BackingItems[i];
            //}
            //return objArr;
        //}
        ICollection<TOutput> IDynParam<TOutput>.GetBackingItems() => this.BackingItems;

        /// <summary>
        /// Finds the underlying object that matches the designated property used to build a ValidateSet attribute.
        /// </summary>
        /// <param name="chosenValue">The value selected after IDynamicParameters has been processed.</param>
        object IDynParam.GetItemFromChosenValue(object chosenValue)
        {
            TOutput outVal = default;
            for (int i = 0; i < this.BackingItems.Count; i++)
            {
                TOutput bi = this.BackingItems[i];
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
                PropertyInfo pi = typeof(TOutput).GetProperty(_mappedProperty, PUB_INST);
                if (pi != null)
                {
                    foreach (TOutput bi in this.BackingItems)
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
        public TOutput GetItemFromChosenValue(object chosenValue)
        {
            TOutput retVal = default;
            for (int i = 0; i < this.BackingItems.Count; i++)
            {
                TOutput bi = this.BackingItems[i];
                if (Convert.ToString(_propertyFunc(bi)).Equals(chosenValue))
                {
                    retVal = bi;
                    break;
                }
            }
            return retVal;
        }

        public IEnumerable<TOutput> GetItemsFromChosenValues(IEnumerable<object> chosenValues)
        {
            return this.BackingItems.Where(x => chosenValues.Any(o => Convert.ToString(_propertyFunc(x)).Equals(o)));
        }

        #endregion
    }
}
