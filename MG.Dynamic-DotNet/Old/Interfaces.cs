using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;

namespace MG.Dynamic
{
    public interface IDynamicDefiner
    {
        string Name { get; }
        Type ParameterType { get; }
        bool Mandatory { get; set; }
        int? Position { get; set; }
        List<string> ValidatedItems { get; }
        List<string> Aliases { get; }
        bool AllowNull { get; set; }
        bool AllowEmptyCollection { get; set; }
        bool AllowEmptyString { get; set; }
        KeyValuePair<int, int>? ValidateCount { get; set; }
        bool ValidateNotNull { get; set; }
        bool ValidateNotNullOrEmpty { get; set; }

        void Clear();
        ParameterAttribute SetParameterAttribute();

        RuntimeDefinedParameter NewParameter();
        RuntimeDefinedParameterDictionary NewDictionary();
        RuntimeDefinedParameterDictionary NewDictionary(RuntimeDefinedParameter[] parameters);
    }
}
