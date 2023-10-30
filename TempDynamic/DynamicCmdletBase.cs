using TempDynamic;
using TempDynamic.Extensions;
using TempDynamic.Extensions.Lists;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace TempDynamic
{
    public class DynamicCmdletBase : PSCmdlet, IDynamicParameters
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PROPERTIES
        protected IDictionary<string, IPowerShellDynamicParameter> DynamicParameters { get; } = new Dictionary<string, IPowerShellDynamicParameter>(1);

        #endregion

        #region DYNAMIC
        public virtual object GetDynamicParameters()
        {
            var dict = new RuntimeDefinedParameterDictionary();
            foreach (IPowerShellDynamicParameter p in this.DynamicParameters.Values)
            {
                dict.Add(p.AsRuntimeParameter());
            }

            return dict;
        }
        protected virtual void DefineDynamicParameters(params IPowerShellDynamicParameter[] parameters)
        {

        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();

        }

        protected override void ProcessRecord()
        {

        }

        protected override void EndProcessing()
        {

        }

        #endregion

        #region BACKEND METHODS


        #endregion
    }
}