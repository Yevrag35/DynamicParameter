using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamic
{
    public class DynParamEquality : EqualityComparer<Parameter>
    {
        public override bool Equals(Parameter x, Parameter y)
        {
            if (x.Name == y.Name && x.Type.Equals(y.Type))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override int GetHashCode(Parameter obj)
        {
            return 0;
        }
    }
}
