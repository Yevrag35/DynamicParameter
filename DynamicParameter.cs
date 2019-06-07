using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic
{
    public class DynamicParameter<T> : IDynParam
    {
        #region FIELDS/CONSTANTS
        private const BindingFlags PUB_INST = BindingFlags.Public | BindingFlags.Instance;
        private List<string> _aliases;
        private List<string> _drives;
        private List<string> _items;
        private List<T> _backingItems;
        private string _mappedProperty;

        #endregion

        #region PROPERTIES
        public List<string> Aliases => _aliases;
        public bool AllowEmptyCollection { get; set; }
        public bool AllowEmptyString { get; set; }
        public bool AllowNull { get; set; }
        public List<T> BackingItems => _backingItems;
        Type IDynParam.BackingItemType => typeof(T);
        public bool DontShow { get; set; }
        public string HelpMessage { get; set; }
        public string HelpMessageBaseName { get; set; }
        public string HelpMessageResourceId { get; set; }
        public bool Mandatory { get; set; }
        public string Name { get; set; }
        public string ParameterSetName { get; set; }
        public Type ParameterType { get; set; }
        public int? Position { get; set; }
        public bool SupportsWildcards { get; set; }
        public KeyValuePair<int, int>? ValidateCount { get; set; }
        public List<string> ValidateDrives => _drives;
        public KeyValuePair<int, int>? ValidateLength { get; set; }
        public List<string> ValidatedItems => _items;
        public bool ValidateNotNull { get; set; }
        public bool ValidateNotNullOrEmpty { get; set; }
        public string ValidatePattern { get; set; }
        public KeyValuePair<int, int>? ValidateRange { get; set; }
        public bool ValidateUserDrive { get; set; }
        public bool ValueFromPipeline { get; set; }
        public bool ValueFromPipelineByPropertyName { get; set; }
        public bool ValueFromRemainingArguments { get; set; }

        #endregion

        #region CONSTRUCTORS
        public DynamicParameter()
        {
            _aliases = new List<string>();
            _drives = new List<string>();
            _items = new List<string>();
            _backingItems = new List<T>();
        }
        public DynamicParameter(string name)
            : this() => this.Name = name;

        public DynamicParameter(string name, Type parameterType)
            : this(name) => this.ParameterType = parameterType;

        public DynamicParameter(string name, Type parameterType, IEnumerable<T> items)
        {
            this.Name = name;
            this.ParameterType = parameterType;
            _aliases = new List<string>();
            _backingItems = new List<T>(items);
            _drives = new List<string>();
            _items = new List<string>();
        }
        public DynamicParameter(string name, IEnumerable<T> items, Func<T, string> validateSetProperty, string mappingProperty, bool parameterTypeIsArray = false)
        {
            this.Name = name;
            _mappedProperty = mappingProperty;
            Type t = typeof(string);
            _aliases = new List<string>();
            _backingItems = new List<T>(items);
            _drives = new List<string>();
            _items = new List<string>(items.Select(validateSetProperty));

            if (parameterTypeIsArray)
                t = t.MakeArrayType();

            this.ParameterType = t;
        }
        public DynamicParameter(string name, IEnumerable<T> items, Func<T, ValueType> validateSetProperty, string mappingProperty, bool parameterTypeIsArray = false)
        {
            this.Name = name;
            _mappedProperty = mappingProperty;
            _aliases = new List<string>();
            _backingItems = new List<T>(items);
            _drives = new List<string>();
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
        //public void Add()

        public RuntimeDefinedParameter AsRuntimeParameter()
        {
            if (string.IsNullOrEmpty(this.Name))
                throw new InvalidCastException("To make a RuntimeDefinedParameter, a parameter name needs to be set.");

            var attCol = new List<Attribute>();

            if (_aliases.Count > 0)
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

            if (this.ValidateUserDrive)
                attCol.Add(new ValidateUserDriveAttribute());

            if (this.ValidateCount.HasValue)
                attCol.Add(new ValidateCountAttribute(this.ValidateCount.Value.Key, this.ValidateCount.Value.Value));

            if (this.ValidateLength.HasValue)
                attCol.Add(new ValidateLengthAttribute(this.ValidateLength.Value.Key, this.ValidateLength.Value.Value));

            if (this.ValidateRange.HasValue)
                attCol.Add(new ValidateRangeAttribute(this.ValidateRange.Value.Key, this.ValidateRange.Value.Value));

            if (_drives.Count > 0)
                attCol.Add(new ValidateDriveAttribute(_drives.ToArray()));

            if (_items.Count > 0)
                attCol.Add(new ValidateSetAttribute(_items.ToArray()));

            attCol.Add(this.MakeParameterAttribute());

            return new RuntimeDefinedParameter(this.Name, this.ParameterType, new Collection<Attribute>(attCol));
        }

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

        #region OPERATORS
        //public static implicit operator RuntimeDefinedParameter(DynamicParameter<T> dynp) => dynp.AsRuntimeParameter();

        #endregion
    }
}
