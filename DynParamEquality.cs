using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dynamic
{
    internal class DynParamEquality : EqualityComparer<Parameter>
    {
        public override bool Equals(Parameter x, Parameter y)
        {
            if (x.Name == y.Name && x.ParameterType.Equals(y.ParameterType))
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
