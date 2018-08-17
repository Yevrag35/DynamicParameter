using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Dynamic
{
    public interface IDynamic : IEquatable<IDynamic>
    {
        string Name { get; set; }
        Type ParameterType { get; set; }
        bool IsSet { get; }
        object Value { get; set; }
        string[] ValidatedItems { get; set; }
        string[] Aliases { get; set; }
        bool AllowNull { get; }
        bool AllowEmptyCollection { get; }
        bool AllowEmptyString { get; }
        bool ValidateNotNull { get; }
        bool ValidateNotNullOrEmpty { get; }

        void Clear();
        void CommitAttributes();
        void SetValidateCount(int minLength, int maxLength);
        T Cast<T>(object o);
        void SetParameterAttributes(IDictionary attributes);
    }
}
