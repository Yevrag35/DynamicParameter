using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic.Parameter
{
    public class RuntimeParameter : IRuntimeParameter
    {
        #region FIELDS/CONSTANTS
        private const BindingFlags PUB_INST = BindingFlags.Public | BindingFlags.Instance;
        private string _key;

        #endregion

        #region PROPERTIES
        public IList<string> Aliases { get; set; } = new List<string>();
        public bool AllowEmptyCollection { get; set; }
        public bool AllowEmptyString { get; set; }
        public bool AllowNull { get; set; }
        public bool DontShow { get; set; }
        public string HelpMessage { get; set; }
        public string HelpMessageBaseName { get; set; }
        public string HelpMessageResourceId { get; set; }
        //public virtual string Key { get; set; }
        string IRuntimeParameter.Key { get { return _key; } set { _key = value; } }
        public bool Mandatory { get; set; }
        public string Name { get; set; }
        public string ParameterSetName { get; set; }
        public virtual Type ParameterType { get; set; }
        public int? Position { get; set; }
        public bool SupportsWildcards { get; set; }
        public (int, int)? ValidateCount { get; set; }
        public (int, int)? ValidateLength { get; set; }
        /// <summary>
        /// A set of strings that must match each parameter argument specified.
        /// </summary>
        public ISet<string> ValidatedItems { get; set; } = new HashSet<string>();
        ICollection<string> IRuntimeParameter.ValidatedItems => this.ValidatedItems;
        public bool ValidateNotNull { get; set; }
        public bool ValidateNotNullOrEmpty { get; set; }
        public string ValidatePattern { get; set; }
        public (object, object)? ValidateRange { get; set; }
        public bool ValueFromPipeline { get; set; }
        public bool ValueFromPipelineByPropertyName { get; set; }
        public bool ValueFromRemainingArguments { get; set; }

        #endregion

        #region CONSTRUCTORS
        //public RuntimeParameter() { }
        public RuntimeParameter(string name) => this.Name = name;

        #endregion

        #region PUBLIC METHODS
        /// <summary>
        /// Converts this instance of <see cref="RuntimeParameter"/> to <see cref="RuntimeDefinedParameter"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">The name of the parameter has not been defined.</exception>
        public virtual RuntimeDefinedParameter AsRuntimeDefinedParameter()
        {
            if (string.IsNullOrEmpty(this.Name))
                throw new InvalidOperationException("To make a RuntimeDefinedParameter, a parameter name needs to be set.");

            else if (this.ParameterType == null)
                throw new InvalidOperationException("To make a RuntimeDefinedParameter, the parameter's type must be defined.");

            var attCol = new List<Attribute>();

            if (this.Aliases != null && this.Aliases.Count > 0)
                attCol.Add(new AliasAttribute(this.Aliases.ToArray()));

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
                attCol.Add(new ValidateCountAttribute(this.ValidateCount.Value.Item1, this.ValidateCount.Value.Item2));

            if (this.ValidateLength.HasValue)
                attCol.Add(new ValidateLengthAttribute(this.ValidateLength.Value.Item1, this.ValidateLength.Value.Item2));

            if (this.ValidateRange.HasValue)
                attCol.Add(new ValidateRangeAttribute(this.ValidateRange.Value.Item1, this.ValidateRange.Value.Item2));

            if (this.ValidatedItems != null && this.ValidatedItems.Count > 0)
                attCol.Add(new ValidateSetAttribute(this.ValidatedItems.ToArray()));

            attCol.Add(this.MakeParameterAttribute());

            return new RuntimeDefinedParameter(this.Name, this.ParameterType, new Collection<Attribute>(attCol));
        }
        public string GetKey(bool useNameIfEmptyKey)
        {
            if (useNameIfEmptyKey)
            {
                return string.IsNullOrEmpty(_key) ? this.Name : _key;
            }
            else
                return _key;
        }
        public virtual ParameterAttribute MakeParameterAttribute()
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
    }
}
