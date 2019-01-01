using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace MG.Dynamic
{
    public interface IDynamic
    {
        string Name { get; set; }
        Type ParameterType { get; set; }
        bool IsSet { get; }
        object Value { get; set; }
        IList<string> ValidatedItems { get; }
        IList<string> Aliases { get; }
        bool AllowNull { get; }
        bool AllowEmptyCollection { get; }
        bool AllowEmptyString { get; }
        bool ValidateNotNull { get; }
        bool ValidateNotNullOrEmpty { get; }

        void Clear();
        void SetValidateCount(int minLength, int maxLength);
        void SetParameterAttributes(IDictionary attributes);
    }
}
