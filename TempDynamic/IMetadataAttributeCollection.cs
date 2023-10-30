using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Internal;
using System.Text;

namespace TempDynamic
{
    public interface IMetadataAttributeCollection : ICollection<Attribute>, IEnumerable<Attribute>
    {
        ParameterAttribute Parameter { get; }

        bool Add(CmdletMetadataAttribute cmdAtt);
        bool Contains<T>() where T : CmdletMetadataAttribute;
        bool Contains(Type cmdletMetadataAttributeType);
        T GetAttribute<T>() where T : CmdletMetadataAttribute;
        bool Remove<T>() where T : CmdletMetadataAttribute;
        bool Remove(Type cmdletMetadataAttributeType);

        Collection<Attribute> ToCollection();
    }
}
