using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Internal;

namespace TempDynamic
{
    internal class CmdletMetadataAttributeEquality : IEqualityComparer<Attribute>, IEqualityComparer<CmdletMetadataAttribute>
    {
        public bool Equals(Attribute x, Attribute y)
        {
            return (x?.Equals(y)).GetValueOrDefault();
        }
        public bool Equals(CmdletMetadataAttribute x, CmdletMetadataAttribute y)
        {
            if (null != x && null != y)
            {
                return x.GetType().Equals(y.GetType());
            }
            else
            {
                return false;
            }
        }
        public int GetHashCode(Attribute obj)
        {
            return obj.GetHashCode();
        }
        public int GetHashCode(CmdletMetadataAttribute obj)
        {
            return obj.GetHashCode();
        }
    }
}
