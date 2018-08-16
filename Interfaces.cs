using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Dynamic
{
    public interface IDynParam
    {
        string Name { get; }
        Type ParameterType { get; }
        IList<string> ValidatedItems { get; }
        string[] Aliases { get; }
        bool AllowNull { get; }

        void AddAttributes(IDictionary attributes);

        void AddValidatedItem(IList<string> valItems);

        void RemoveValidatedItem(string[] remItems);

        void AddAliases(IList<string> aliases);
    }
}
