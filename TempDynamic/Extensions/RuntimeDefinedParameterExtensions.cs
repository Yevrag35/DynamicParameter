using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace TempDynamic.Extensions
{
    public static class RuntimeDefinedParameterExtensions
    {
        public static RuntimeDefinedParameter AllowEmptyCollection(this RuntimeDefinedParameter parameter, bool toggle = true)
        {
            if (toggle)
                parameter.Attributes.Add(new AllowEmptyCollectionAttribute());

            return parameter;
        }

        public static RuntimeDefinedParameter AllowEmptyString(this RuntimeDefinedParameter parameter, bool toggle = true)
        {
            if (toggle)
                parameter.Attributes.Add(new AllowEmptyStringAttribute());

            return parameter;
        }

        public static RuntimeDefinedParameter AllowNull(this RuntimeDefinedParameter parameter, bool toggle = true)
        {
            if (toggle)
                parameter.Attributes.Add(new AllowNullAttribute());

            return parameter;
        }

        public static RuntimeDefinedParameter DontShow(this RuntimeDefinedParameter parameter, bool toggle = true)
        {
            if (toggle)
                parameter.
        }
    }
}
