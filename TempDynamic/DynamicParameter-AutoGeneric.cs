using MG.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;

namespace TempDynamic
{
    public class DynamicParameter<T, TValue> : IPowerShellDynamicParameter<TValue>
        where TValue : IConvertible
    {
        private readonly DynamicParameter<TValue> BackingParameter;
        private readonly ManagedKeySortedList<TValue, T> _validatedItems;
        private bool _forceArrayType;

        public List<string> Aliases => BackingParameter.Aliases;
        public bool AllowEmptyCollection
        {
            get => BackingParameter.AllowEmptyCollection;
            set => BackingParameter.AllowEmptyCollection = value;
        }
        public bool AllowEmptyString
        {
            get => BackingParameter.AllowEmptyString;
            set => BackingParameter.AllowEmptyString = value;
        }
        public bool AllowNull
        {
            get => BackingParameter.AllowNull;
            set => BackingParameter.AllowNull = value;
        }
        public bool DontShow
        {
            get => BackingParameter.DontShow;
            set => BackingParameter.DontShow = value;
        }
        public string HelpMessage
        {
            get => BackingParameter.HelpMessage;
            set => BackingParameter.HelpMessage = value;
        }
        public string HelpMessageBaseName
        {
            get => BackingParameter.HelpMessageBaseName;
            set => BackingParameter.HelpMessageBaseName = value;
        }
        public string HelpMessageResourceId
        {
            get => BackingParameter.HelpMessageResourceId;
            set => BackingParameter.HelpMessageResourceId = value;
        }
        public bool Mandatory
        {
            get => BackingParameter.Mandatory;
            set => BackingParameter.Mandatory = value;
        }
        public string Name
        {
            get => BackingParameter.Name;
            set => BackingParameter.Name = value;
        }
        public bool ParameterIsArray
        {
            get => _forceArrayType || typeof(TValue).IsArray;
            set => _forceArrayType = value;
        }
        public string ParameterSetName
        {
            get => BackingParameter.ParameterSetName;
            set => BackingParameter.ParameterSetName = value;
        }
        public int Position
        {
            get => BackingParameter.Position;
            set => BackingParameter.Position = value;
        }
        public bool SupportsWildcards
        {
            get => BackingParameter.SupportsWildcards;
            set => BackingParameter.SupportsWildcards = value;
        }
        public Range? ValidateCount
        {
            get => BackingParameter.ValidateCount;
            set => BackingParameter.ValidateCount = value;
        }
        public Range? ValidateLength
        {
            get => BackingParameter.ValidateLength;
            set => BackingParameter.ValidateLength = value;
        }
        public IList<T> ValidatedItems => _validatedItems;
        public bool ValidateNotNull
        {
            get => BackingParameter.ValidateNotNull;
            set => BackingParameter.ValidateNotNull = value;
        }
        public bool ValidateNotNullOrEmpty
        {
            get => BackingParameter.ValidateNotNullOrEmpty;
            set => BackingParameter.ValidateNotNullOrEmpty = value;
        }
        public bool ValueFromPipeline
        {
            get => BackingParameter.ValueFromPipeline;
            set => BackingParameter.ValueFromPipeline = true;
        }
        public bool ValueFromPipelineByPropertyName
        {
            get => BackingParameter.ValueFromPipelineByPropertyName;
            set => BackingParameter.ValueFromPipelineByPropertyName = value;
        }
        public bool ValueFromRemainingArguments
        {
            get => BackingParameter.ValueFromRemainingArguments;
            set => BackingParameter.ValueFromRemainingArguments = value;
        }

        public DynamicParameter(Func<T, TValue> itemSelector)
            : this(null, itemSelector)
        {
        }
        public DynamicParameter(string name, Func<T, TValue> itemSelector)
        {
            BackingParameter = new DynamicParameter<TValue>(name);
            _validatedItems = new ManagedKeySortedList<TValue, T>(itemSelector);
        }
        public DynamicParameter(string name, IEnumerable<CmdletMetadataAttribute> attributes, Func<T, TValue> itemSelector)
        {
            BackingParameter = new DynamicParameter<TValue>(name, 
                attributes.Where(x => !typeof(ValidateSetAttribute).Equals(x?.GetType())));

            _validatedItems = new ManagedKeySortedList<TValue, T>(itemSelector);
        }

        public static implicit operator RuntimeDefinedParameter(DynamicParameter<T, TValue> rp)
        {
            rp.BackingParameter.ValidatedItems.Clear();
            rp.BackingParameter.ValidatedItems.UnionWith(rp._validatedItems.Keys.Select(x => Convert.ToString(x)));

            RuntimeDefinedParameter rtParam = rp.BackingParameter;
            if (rp.ParameterIsArray && !rp.BackingParameter.DefinedParameter.ParameterType.IsArray)
                rtParam.ParameterType = rtParam.ParameterType.MakeArrayType();
            
            return rtParam;
        }

        public RuntimeDefinedParameter AsRuntimeParameter()
        {
            return this;
        }

        object IPowerShellDynamicParameter.GetChosenValue()
        {
            return ((IPowerShellDynamicParameter)BackingParameter)?.GetChosenValue();
        }
        /// <summary>
        /// Retrieves the selected parameter value.
        /// </summary>
        /// <returns>
        ///     The single value selected by the parameter casted to the parameter type <typeparamref name="T"/>.  If no value
        ///     was selected, the default value for <typeparamref name="T"/> is returned.
        /// </returns>
        public TValue GetChosenValue()
        {
            return BackingParameter.GetChosenValue();
        }
        public TValue[] GetChosenValues()
        {
            return BackingParameter.GetChosenValues();
        }
        public T GetMatchingSource()
        {
            TValue chosenValue = this.GetChosenValue();
            return _validatedItems[chosenValue];
        }
        public T[] GetMatchingSources()
        {
            TValue[] chosenValues = this.GetChosenValues();
            if (null == chosenValues || chosenValues.Length <= 0)
                return new T[0];

            var newArr = new T[chosenValues.Length];
            for (int i = 0; i < chosenValues.Length; i++)
            {
                newArr[i] = _validatedItems[chosenValues[i]];
            }

            return newArr;
        }
        protected static void CopyValidatedItems(DynamicParameter<TValue> parameter, ICollection<IConvertible> keys)
        {
            if (null == keys)
                return;

            parameter.ValidatedItems.Clear();
            parameter.ValidatedItems.UnionWith(keys.Select(x => Convert.ToString(x)));
        }
    }
}
