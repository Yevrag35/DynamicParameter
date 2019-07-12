using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic
{
    /// <summary>
    /// Defines properties and methods to store underlying values that 
    /// create a dynamic 'ValidateSet' off of one of its properties to be used within a <see cref="DynamicLibrary"/>.
    /// </summary>
    public interface IDynParam
    {
        /// <summary>
        /// Declares alternative names for the parameter.
        /// </summary>
        List<string> Aliases { get; }

        /// <summary>
        /// Declares an empty collection can be used as an argument to a mandatory collection parameter.
        /// </summary>
        bool AllowEmptyCollection { get; set; }

        /// <summary>
        /// Declares an empty string can be used as an argument to a mandatory string parameter.
        /// </summary>
        bool AllowEmptyString { get; set; }

        /// <summary>
        /// Declares a NULL can be used as an argument to a mandatory parameter.
        /// </summary>
        bool AllowNull { get; set; }

        /// <summary>
        /// The underlying type of the backend item collection that signifies this class's generic constraint.
        /// </summary>
        Type BackingItemType { get; }

        /// <summary>
        /// Declares that the parameter will be hidden from the console unless typed explicitly.
        /// </summary>
        bool DontShow { get; set; }

        /// <summary>
        /// Gets and sets a short description for this parameter, suitable for presentation as a tooltip.
        /// </summary>
        string HelpMessage { get; set; }

        /// <summary>
        /// Gets and sets the base name of the resource for a help message. 
        /// When this field is speicifed, HelpMessageResourceId must also be specified.
        /// </summary>
        string HelpMessageBaseName { get; set; }

        /// <summary>
        /// Gets and sets the Id of the resource for a help message. 
        /// When this field is speicifed, HelpMessageBaseName must also be specified.
        /// </summary>
        string HelpMessageResourceId { get; set; }

        /// <summary>
        /// Gets and sets a flag specifying if this parameter is Mandatory. 
        /// When it is not specified, false is assumed and the parameter is considered optional.
        /// </summary>
        bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the name of the parameter.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets and sets the name of the parameter set this parameter belongs to. 
        /// When it is not specified, ParameterAttribute.AllParameterSets is assumed.
        /// </summary>
        string ParameterSetName { get; set; }

        /// <summary>
        /// Gets or sets the type of the parameter.
        /// </summary>
        Type ParameterType { get; set; }

        /// <summary>
        /// Gets and sets the parameter position. If not set, the parameter is named.
        /// </summary>
        int? Position { get; set; }

        /// <summary>
        /// Declares that this parameter supports wildcards.
        /// </summary>
        bool SupportsWildcards { get; set; }

        /// <summary>
        /// Declares that this parameter argument count must be in the specified range specified by the key (MinCount) and value (MaxCount).
        /// </summary>
        KeyValuePair<int, int>? ValidateCount { get; set; }

        /// <summary>
        /// Declares that the length of each parameter argument's Length must fall in the range specified by the key (MinLength) and value (MaxLength).
        /// </summary>
        KeyValuePair<int, int>? ValidateLength { get; set; }

        /// <summary>
        /// Declares a collection of strings that each parameter argument is present in this specific collection.
        /// </summary>
        List<string> ValidatedItems { get; }

        /// <summary>
        /// Validates that the parameters's argument is not null.
        /// </summary>
        bool ValidateNotNull { get; set; }

        /// <summary>
        /// Validates that the parameters's argument is not null, is not an empty string, and is not an empty collection.
        /// </summary>
        bool ValidateNotNullOrEmpty { get; set; }

        /// <summary>
        /// Validates that each parameter argument matches specified the RegexPattern.
        /// </summary>
        string ValidatePattern { get; set; }

        /// <summary>
        /// Declares that each parameter argument must fall in the range specified by the key (MinRange) and value (MaxRange).
        /// </summary>
        KeyValuePair<int, int>? ValidateRange { get; set; }

        /// <summary>
        /// Gets and sets a flag that specifies that this parameter can take values from the incoming pipeline object. 
        /// When it is not specified, false is assumed.
        /// </summary>
        bool ValueFromPipeline { get; set; }

        /// <summary>
        /// Gets and sets a flag that specifies that this parameter can take values from
        /// a property in the incoming pipeline object with the same name as the parameter.
        /// When it is not specified, false is assumed.
        /// </summary>
        bool ValueFromPipelineByPropertyName { get; set; }

        /// <summary>
        /// Gets and sets a flag that specifies that the remaining command line parameters
        /// should be associated with this parameter in the form of an array. When it is
        /// not specified, false is assumed.
        /// </summary>
        bool ValueFromRemainingArguments { get; set; }

        /// <summary>
        /// Converts the inherited class into its RuntimeDefinedParameter equivalent.
        /// </summary>
        RuntimeDefinedParameter AsRuntimeParameter();

        /// <summary>
        /// Finds the underlying object that matches the designated property used to build a ValidateSet attribute.
        /// </summary>
        /// <param name="chosenValue">The value selected after IDynamicParameters has been processed.</param>
        object GetItemFromChosenValue(object chosenValue);

        /// <summary>
        /// Finds the underlying objects that match the designated property used to build a ValidateSet attribute.
        /// </summary>
        /// <param name="chosenValues">The values selected after IDynamicParameters has been processed.</param>
        IEnumerable<object> GetItemsFromChosenValues(object[] chosenValues);

        /// <summary>
        /// Retrieves all the underlying objects that were used to build the ValidateSet.
        /// </summary>
        object[] GetBackingItems();
    }
}