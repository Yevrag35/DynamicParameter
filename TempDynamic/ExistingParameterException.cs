using System;
using System.Collections.Generic;
using System.Management.Automation.Internal;
using System.Text;

namespace TempDynamic
{
    public class ExistingParameterException<TAtt> : ArgumentException
        where TAtt : Attribute
    {
        private const string DEFAULT = "{0} already contains a {1}";

        public ExistingParameterException()
            : base(string.Format(DEFAULT, nameof(CmdletMetadataCollection), typeof(TAtt).Name))
        {
        }
        public ExistingParameterException(TAtt attribute)
            : base(string.Format(DEFAULT, nameof(CmdletMetadataCollection), attribute?.GetType().Name))
        {
        }
    }
}
