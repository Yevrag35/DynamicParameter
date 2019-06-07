using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Reflection;

namespace MG.Dynamic
{
    public interface IDynParam
    {
        List<string> Aliases { get; }
        bool AllowEmptyCollection { get; set; }
        bool AllowEmptyString { get; set; }
        bool AllowNull { get; set; }
        Type BackingItemType { get; }
        bool DontShow { get; set; }
        string HelpMessage { get; set; }
        string HelpMessageBaseName { get; set; }
        string HelpMessageResourceId { get; set; }
        bool Mandatory { get; set; }
        string Name { get; set; }
        string ParameterSetName { get; set; }
        Type ParameterType { get; set; }
        int? Position { get; set; }
        bool SupportsWildcards { get; set; }
        KeyValuePair<int, int>? ValidateCount { get; set; }
        List<string> ValidateDrives { get; }
        KeyValuePair<int, int>? ValidateLength { get; set; }
        List<string> ValidatedItems { get; }
        bool ValidateNotNull { get; set; }
        bool ValidateNotNullOrEmpty { get; set; }
        string ValidatePattern { get; set; }
        KeyValuePair<int, int>? ValidateRange { get; set; }
        bool ValidateUserDrive { get; set; }
        bool ValueFromPipeline { get; set; }
        bool ValueFromPipelineByPropertyName { get; set; }
        bool ValueFromRemainingArguments { get; set; }

        RuntimeDefinedParameter AsRuntimeParameter();
        object GetItemFromChosenValue(object chosenValue);
        IEnumerable<object> GetItemsFromChosenValues(object[] chosenValues);
    }
}