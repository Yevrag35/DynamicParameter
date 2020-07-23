using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic.Parameter
{
    public class RuntimeParameter : IDynParam
    {
        #region FIELDS/CONSTANTS
        private const BindingFlags PUB_INST = BindingFlags.Public | BindingFlags.Instance;

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
        public string Key { get; set; }
        public bool Mandatory { get; set; }
        public string Name { get; set; }
        public string ParameterSetName { get; set; }
        public Type ParameterType { get; set; }
        public int? Position { get; set; }
        public bool SupportsWildcards { get; set; }
        public (int, int)? ValidateCount { get; set; }
        public (int, int)? ValidateLength { get; set; }
        /// <summary>
        /// A set of strings that must match each parameter argument specified.
        /// </summary>
        public ISet<string> ValidatedItems { get; set; } = new HashSet<string>();
        ICollection<string> IDynParam.ValidatedItems => this.ValidatedItems;
        public bool ValidateNotNull { get; set; }
        public bool ValidateNotNullOrEmpty { get; set; }
        public string ValidatePattern { get; set; }
        public (object, object)? ValidateRange { get; set; }
        public bool ValueFromPipeline { get; set; }
        public bool ValueFromPipelineByPropertyName { get; set; }
        public bool ValueFromRemainingArguments { get; set; }

        #endregion

        #region CONSTRUCTORS
        public RuntimeParameter() { }
        public RuntimeParameter(string name) => this.Name = name;

        #endregion

        #region PUBLIC METHODS


        #endregion
    }
}
