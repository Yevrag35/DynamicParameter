using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Dynamic
{
     interface IDynamic
    {
        string Name { get; }
        Type ParameterType { get; }
        bool IsSet { get; }
        object Value { get; set; }
        string[] ValidatedItems { get; }
        string[] Aliases { get; }
        bool AllowNull { get; set; }
        bool AllowEmptyCollection { get; set; }
        bool AllowEmptyString { get; set; }
        bool ValidateNotNull { get; set; }
        bool ValidateNotNullOrEmpty { get; set; }

        void Clear();
    }
}
