using MG.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Reflection;

namespace TempDynamic
{
    public class DynamicParameter<T, TValue> : DynamicParameter<TValue>
    {
        public DynamicParameter()
            : this(null)
        {
        }
        public DynamicParameter(string name)
            : base(name)
        {
        }
        public DynamicParameter(string name, IEnumerable<CmdletMetadataAttribute> attributes)
            : base(name, attributes)
        {
        }

        public static implicit operator RuntimeDefinedParameter(DynamicParameter<T, TValue> rp)
        {
            if (!rp.Attributes.Contains<AliasAttribute>() && TryCopyAliases(rp.Aliases, out AliasAttribute aliases))
                rp.Attributes.Add(aliases);

            if (TryCopyValidatedItems(rp.ValidatedItems, out ValidateSetAttribute attribute) && !rp.Attributes.Contains<ValidateSetAttribute>())
                rp.Attributes.Add(attribute);

            rp.BackingParameter = new RuntimeDefinedParameter(rp.Name, typeof(T), rp.Attributes);
            return rp.BackingParameter;
        }
    }
}
