using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace MG.Dynamic.Parameter
{
    /// <summary>
    /// An interface exposing properties and methods for constructing a <see cref="RuntimeDefinedParameter"/>.
    /// </summary>
    public interface IRuntimeParameter
    {
        /// <summary>
        /// A collection of alternative names for the parameter.
        /// </summary>
        IList<string> Aliases { get; }

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
        /// Declares that the parameter will be hidden from the console unless typed explicitly.
        /// </summary>
        bool DontShow { get; set; }

        /// <summary>
        /// Gets and sets a short description for this parameter, suitable for presentation as a tooltip.
        /// </summary>
        string HelpMessage { get; set; }

        /// <summary>
        /// The base name of the resource for a help message.
        /// </summary>
        /// <remarks>
        ///     When this field is speicifed, HelpMessageResourceId must also be specified.
        /// </remarks>
        string HelpMessageBaseName { get; set; }

        /// <summary>
        /// The ID of the resource for a help message. 
        /// When this field is speicifed, HelpMessageBaseName must also be specified.
        /// </summary>
        string HelpMessageResourceId { get; set; }

        /// <summary>
        /// The key used in the <see cref="DynamicLibrary"/> to retrieve this parameter.
        /// </summary>
        /// <remarks>
        ///     This key needs to be added manually when used in a standard
        ///     <see cref="RuntimeDefinedParameterDictionary"/>.
        /// </remarks>
        string Key { get; set; }

        /// <summary>
        /// A flag specifying if this parameter is Mandatory.
        /// </summary>
        /// <remarks>
        ///     When not specified, <see langword="false"/> is assumed and the parameter is considered optional.
        /// </remarks>
        bool Mandatory { get; set; }

        /// <summary>
        /// The name of the parameter.
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
        (int, int)? ValidateCount { get; set; }

        /// <summary>
        /// Declares that the length of each parameter argument's Length must fall in the range specified by the key (MinLength) and value (MaxLength).
        /// </summary>
        (int, int)? ValidateLength { get; set; }

        /// <summary>
        /// Declares a collection of strings that each parameter argument is present in this specific collection.
        /// </summary>
        ICollection<string> ValidatedItems { get; }

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
        (object, object)? ValidateRange { get; set; }

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
        RuntimeDefinedParameter AsRuntimeDefinedParameter();
        string GetKey(bool nameIfEmpty);
        ParameterAttribute MakeParameterAttribute();
    }
}
