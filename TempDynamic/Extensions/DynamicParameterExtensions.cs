using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace TempDynamic.Extensions
{
    public static class DynamicParameterExtensions
    {
        public static DynamicParameter AddAlias(this DynamicParameter dp, string alias)
        {
            dp.Aliases.Add(alias);
            return dp;
        }
        public static DynamicParameter AddAlias(this DynamicParameter dp, IEnumerable<string> aliases)
        {
            dp.Aliases.AddRange(aliases);
            return dp;
        }
        public static DynamicParameter SetAllowEmptyCollection(this DynamicParameter dp, bool toggle = true)
        {
            dp.AllowEmptyCollection = toggle;
            return dp;
        }
        public static DynamicParameter SetAllowEmptyString(this DynamicParameter dp, bool toggle = true)
        {
            dp.AllowEmptyString = toggle;
            return dp;
        }
        public static DynamicParameter SetAllowNull(this DynamicParameter dp, bool toggle = true)
        {
            dp.AllowNull = toggle;
            return dp;
        }
        public static DynamicParameter SetDontShow(this DynamicParameter dp, bool toggle = true)
        {
            dp.AllowNull = toggle;
            return dp;
        }
        public static DynamicParameter SetHelpMessage(this DynamicParameter dp, string message)
        {
            dp.HelpMessage = message;
            return dp;
        }
        public static DynamicParameter SetHelpMessageBaseName(this DynamicParameter dp, string messageBaseName)
        {
            dp.HelpMessageBaseName = messageBaseName;
            return dp;
        }
        public static DynamicParameter SetHelpMessageResourceId(this DynamicParameter dp, string resourceId)
        {
            dp.HelpMessageResourceId = resourceId;
            return dp;
        }
        public static DynamicParameter SetMandatory(this DynamicParameter dp, bool toggle = true)
        {
            dp.Mandatory = toggle;
            return dp;
        }
        public static DynamicParameter SetName(this DynamicParameter dp, string name)
        {
            dp.Name = name;
            return dp;
        }
        public static DynamicParameter SetParameterSetName(this DynamicParameter dp, string setName)
        {
            dp.ParameterSetName = setName;
            return dp;
        }
        public static DynamicParameter SetPosition(this DynamicParameter dp, int position)
        {
            dp.Position = position;
            return dp;
        }
        public static DynamicParameter SetSupportsWildcards(this DynamicParameter dp, bool toggle = true)
        {
            dp.SupportsWildcards = toggle;
            return dp;
        }
        public static DynamicParameter SetValidateCount(this DynamicParameter dp, int min, int max)
        {
            return SetValidateCount(dp, new Range(min, max));
        }
        public static DynamicParameter SetValidateCount(this DynamicParameter dp, Range range)
        {
            dp.ValidateCount = range;
            return dp;
        }
        public static DynamicParameter SetValidateLength(this DynamicParameter dp, int min, int max)
        {
            return SetValidateLength(dp, new Range(min, max));
        }
        public static DynamicParameter SetValidateLength(this DynamicParameter dp, Range range)
        {
            dp.ValidateLength = range;
            return dp;
        }
        public static DynamicParameter SetValidateNotNull(this DynamicParameter dp, bool toggle = true)
        {
            dp.ValidateNotNull = toggle;
            return dp;
        }
        public static DynamicParameter SetValidateNotNullOrEmpty(this DynamicParameter dp, bool toggle = true)
        {
            dp.ValidateNotNullOrEmpty = toggle;
            return dp;
        }
        public static DynamicParameter SetValueFromPipeline(this DynamicParameter dp, bool toggle = true)
        {
            dp.ValueFromPipeline = toggle;
            return dp;
        }
        public static DynamicParameter SetValueFromPipelineByPropertyName(this DynamicParameter dp, bool toggle = true)
        {
            dp.ValueFromPipelineByPropertyName = toggle;
            return dp;
        }
        public static DynamicParameter SetValueFromRemainingArguments(this DynamicParameter dp, bool toggle = true)
        {
            dp.ValueFromRemainingArguments = toggle;
            return dp;
        }
    }
}
