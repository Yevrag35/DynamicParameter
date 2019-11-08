using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic
{
    /// <summary>
    /// A class to be used when constructing a <see cref="DynamicLibrary"/> or <see cref="RuntimeDefinedParameterDictionary"/>
    /// without the need for a collection of attributes.  This class can store underlying values that create a dynamic 
    /// 'ValidateSet' off of one of its properties.  The generic type is of the underlying items type.
    /// </summary>
    /// <typeparam name="T">The type of the underlying items for the ValidateSet.</typeparam>
    public class DynamicParameter<T> : IDynParam
    {
        #region FIELDS/CONSTANTS
        private const BindingFlags PUB_INST = BindingFlags.Public | BindingFlags.Instance;
        private List<string> _aliases;
        private List<string> _items;
        private List<T> _backingItems;
        private string _mappedProperty;

        #endregion

        #region PROPERTIES
        /// <summary>
        /// Declares alternative names for the parameter.
        /// </summary>
        public List<string> Aliases => _aliases;

        /// <summary>
        /// Declares an empty collection can be used as an argument to a mandatory collection parameter.
        /// </summary>
        public bool AllowEmptyCollection { get; set; }

        /// <summary>
        /// Declares an empty string can be used as an argument to a mandatory string parameter.
        /// </summary>
        public bool AllowEmptyString { get; set; }

        /// <summary>
        /// Declares a NULL can be used as an argument to a mandatory parameter.
        /// </summary>
        public bool AllowNull { get; set; }

        
        public List<T> BackingItems => _backingItems;

        /// <summary>
        /// The underlying type of the backend item collection that signifies this class's generic constraint.
        /// </summary>
        Type IDynParam.BackingItemType => typeof(T);

        /// <summary>
        /// Declares that the parameter will be hidden from the console unless typed explicitly.
        /// </summary>
        public bool DontShow { get; set; }

        /// <summary>
        /// Gets and sets a short description for this parameter, suitable for presentation as a tooltip.
        /// </summary>
        public string HelpMessage { get; set; }

        /// <summary>
        /// Gets and sets the base name of the resource for a help message. 
        /// When this field is speicifed, HelpMessageResourceId must also be specified.
        /// </summary>
        public string HelpMessageBaseName { get; set; }

        /// <summary>
        /// Gets and sets the Id of the resource for a help message. 
        /// When this field is speicifed, HelpMessageBaseName must also be specified.
        /// </summary>
        public string HelpMessageResourceId { get; set; }

        /// <summary>
        /// Gets and sets a flag specifying if this parameter is Mandatory. 
        /// When it is not specified, false is assumed and the parameter is considered optional.
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets and sets the name of the parameter set this parameter belongs to. 
        /// When it is not specified, ParameterAttribute.AllParameterSets is assumed.
        /// </summary>
        public string ParameterSetName { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        public Type ParameterType { get; set; }

        /// <summary>
        /// Gets and sets the parameter position. If not set, the parameter is named.
        /// </summary>
        public int? Position { get; set; }

        /// <summary>
        /// Declares that this parameter supports wildcards.
        /// </summary>
        public bool SupportsWildcards { get; set; }

        /// <summary>
        /// Declares that this parameter argument count must be in the specified range specified by the key (MinCount) and value (MaxCount).
        /// </summary>
        public KeyValuePair<int, int>? ValidateCount { get; set; }

        /// <summary>
        /// Declares that the length of each parameter argument's Length must fall in the range specified by the key (MinLength) and value (MaxLength).
        /// </summary>
        public KeyValuePair<int, int>? ValidateLength { get; set; }

        /// <summary>
        /// Declares a collection of strings that each parameter argument is present in this specific collection.
        /// </summary>
        public List<string> ValidatedItems => _items;

        /// <summary>
        /// Validates that the parameters's argument is not null.
        /// </summary>
        public bool ValidateNotNull { get; set; }

        /// <summary>
        /// Validates that the parameters's argument is not null, is not an empty string, and is not an empty collection.
        /// </summary>
        public bool ValidateNotNullOrEmpty { get; set; }

        /// <summary>
        /// Validates that each parameter argument matches specified the RegexPattern.
        /// </summary>
        public string ValidatePattern { get; set; }

        /// <summary>
        /// Declares that each parameter argument must fall in the range specified by the key (MinRange) and value (MaxRange).
        /// </summary>
        public KeyValuePair<int, int>? ValidateRange { get; set; }

        /// <summary>
        /// Gets and sets a flag that specifies that this parameter can take values from the incoming pipeline object. 
        /// When it is not specified, false is assumed.
        /// </summary>
        public bool ValueFromPipeline { get; set; }

        /// <summary>
        /// Gets and sets a flag that specifies that this parameter can take values from
        /// a property in the incoming pipeline object with the same name as the parameter.
        /// When it is not specified, false is assumed.
        /// </summary>
        public bool ValueFromPipelineByPropertyName { get; set; }

        /// <summary>
        /// Gets and sets a flag that specifies that the remaining command line parameters
        /// should be associated with this parameter in the form of an array. When it is
        /// not specified, false is assumed.
        /// </summary>
        public bool ValueFromRemainingArguments { get; set; }

        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Initializes a blank instance of a DynamicParameter.
        /// </summary>
        public DynamicParameter()
        {
            _aliases = new List<string>();
            _items = new List<string>();
            _backingItems = new List<T>();
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
            _aliases = new List<string>();
            _backingItems = new List<T>(items);
            _items = new List<string>();
        }

        public DynamicParameter(string name, bool parameterTypeIsArray, IEnumerable<T> items, Expression<Func<T, IConvertible>> propertyExpression)
        {
            if (propertyExpression.Body is MemberExpression memEx)
            {
                this.Name = name;
                _mappedProperty = memEx.Member.Name;
                Func<T, IConvertible> func = propertyExpression.Compile();
                _backingItems = new List<T>(items);
                var convertibles = new List<IConvertible>(items.Select(func));
                _items = new List<string>(convertibles.Count);
                convertibles.ForEach((ic) =>
                {
                    _items.Add(Convert.ToString(ic));
                });

                Type t = typeof(string);
                if (parameterTypeIsArray)
                    t = t.MakeArrayType();

                this.ParameterType = t;
            }
            else
                throw new ArgumentException("propertyExpression is not a valid member expression");
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
            _aliases = new List<string>();
            _backingItems = new List<T>(items);
            _items = new List<string>(items.Select(validateSetProperty));

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
            _aliases = new List<string>();
            _backingItems = new List<T>(items);
            _items = new List<string>();
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
        /// Converts the inherited class into its RuntimeDefinedParameter equivalent.
        /// </summary>
        public RuntimeDefinedParameter AsRuntimeParameter()
        {
            if (string.IsNullOrEmpty(this.Name))
                throw new InvalidCastException("To make a RuntimeDefinedParameter, a parameter name needs to be set.");

            var attCol = new List<Attribute>();

            if (_aliases != null && _aliases.Count > 0)
                attCol.Add(new AliasAttribute(_aliases.ToArray()));
            
            if (this.AllowEmptyCollection)
                attCol.Add(new AllowEmptyCollectionAttribute());

            if (this.AllowEmptyString)
                attCol.Add(new AllowEmptyStringAttribute());

            if (this.AllowNull)
                attCol.Add(new AllowNullAttribute());

            if (this.SupportsWildcards)
                attCol.Add(new SupportsWildcardsAttribute());

            if (this.ValidateNotNull)
                attCol.Add(new ValidateNotNullAttribute());

            if (this.ValidateNotNullOrEmpty)
                attCol.Add(new ValidateNotNullOrEmptyAttribute());

            if (this.ValidateCount.HasValue)
                attCol.Add(new ValidateCountAttribute(this.ValidateCount.Value.Key, this.ValidateCount.Value.Value));

            if (this.ValidateLength.HasValue)
                attCol.Add(new ValidateLengthAttribute(this.ValidateLength.Value.Key, this.ValidateLength.Value.Value));

            if (this.ValidateRange.HasValue)
                attCol.Add(new ValidateRangeAttribute(this.ValidateRange.Value.Key, this.ValidateRange.Value.Value));

            if (_items != null && _items.Count > 0)
                attCol.Add(new ValidateSetAttribute(_items.ToArray()));

            attCol.Add(this.MakeParameterAttribute());

            return new RuntimeDefinedParameter(this.Name, this.ParameterType, new Collection<Attribute>(attCol));
        }

        /// <summary>
        /// Retrieves all the underlying objects that were used to build the ValidateSet.
        /// </summary>
        public object[] GetBackingItems()
        {
            var objArr = new object[_backingItems.Count];
            for (int i = 0; i < _backingItems.Count; i++)
            {
                objArr[i] = _backingItems[i];
            }
            return objArr;
        }

        /// <summary>
        /// Finds the underlying object that matches the designated property used to build a ValidateSet attribute.
        /// </summary>
        /// <param name="chosenValue">The value selected after IDynamicParameters has been processed.</param>
        object IDynParam.GetItemFromChosenValue(object chosenValue)
        {
            T outVal = default;
            if (!string.IsNullOrEmpty(_mappedProperty))
            {
                PropertyInfo pi = typeof(T).GetProperty(_mappedProperty, PUB_INST);
                if (pi != null)
                {
                    for (int i = 0; i < _backingItems.Count; i++)
                    {
                        T bi = _backingItems[i];
                        if (pi.GetValue(bi).Equals(chosenValue))
                        {
                            outVal = bi;
                            break;
                        }
                    }
                }
            }
            return outVal;
        }

        /// <summary>
        /// Finds the underlying objects that match the designated property used to build a ValidateSet attribute.
        /// </summary>
        /// <param name="chosenValues">The values selected after IDynamicParameters has been processed.</param>
        IEnumerable<object> IDynParam.GetItemsFromChosenValues(object[] chosenValues)
        {
            var outTs = new List<object>(chosenValues.Length);
            if (!string.IsNullOrEmpty(_mappedProperty))
            {
                PropertyInfo pi = typeof(T).GetProperty(_mappedProperty, PUB_INST);
                if (pi != null)
                {
                    for (int i =  0; i < _backingItems.Count; i++)
                    {
                        T bi = _backingItems[i];
                        
                        for (int i2 = 0; i2 < chosenValues.Length; i2++)
                        {
                            if (pi.GetValue(bi).Equals(chosenValues[i2]))
                            {
                                outTs.Add(bi);
                            }
                        }
                    }
                }
            }
            return outTs;
        }

        /// <summary>
        /// Finds the underlying object that matches the designated property used to build a ValidateSet attribute and
        /// casts the result as the class's generic type.
        /// </summary>
        /// <param name="chosenValue">The value selected after IDynamicParameters has been processed.</param>
        /// <returns></returns>
        public T GetItemFromChosenValue(object chosenValue)
        {
            if (string.IsNullOrEmpty(_mappedProperty))
                throw new InvalidOperationException("No mapped property is present.");

            T retVal = default;
            object outVal = ((IDynParam)this).GetItemFromChosenValue(chosenValue);
            if (outVal != null)
                retVal = (T)outVal;

            return retVal;
        }

        #region PRIVATE METHODS
        private List<G> Cast<G>(dynamic[] os)
        {
            var gList = new List<G>(os.Length);
            for (int i = 0; i < os.Length; i++)
            {
                dynamic o = os[i];
                gList.Add((G)o);
            }
            return gList;
        }

        private ParameterAttribute MakeParameterAttribute()
        {
            var pAtt = new ParameterAttribute
            {
                DontShow = this.DontShow,
                Mandatory = this.Mandatory,
                ValueFromPipeline = this.ValueFromPipeline,
                ValueFromPipelineByPropertyName = this.ValueFromPipelineByPropertyName,
                ValueFromRemainingArguments = this.ValueFromRemainingArguments
            };
            if (!string.IsNullOrEmpty(this.HelpMessage))
                pAtt.HelpMessage = this.HelpMessage;

            if (!string.IsNullOrEmpty(this.HelpMessageBaseName))
                pAtt.HelpMessageBaseName = this.HelpMessageBaseName;

            if (!string.IsNullOrEmpty(this.HelpMessageResourceId))
                pAtt.HelpMessageResourceId = this.HelpMessageResourceId;

            if (!string.IsNullOrEmpty(this.ParameterSetName))
                pAtt.ParameterSetName = this.ParameterSetName;

            if (this.Position.HasValue)
                pAtt.Position = this.Position.Value;

            return pAtt;
        }

        #endregion

        #endregion
    }
}
