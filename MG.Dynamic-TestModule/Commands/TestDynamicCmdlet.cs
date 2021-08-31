//using MG.Dynamic.Library;
//using MG.Dynamic.Parameter;
using TempDynamic;
using TempDynamic.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace MG.Dynamic.Tests.Module.Commands
{
    [Cmdlet(VerbsDiagnostic.Test, "Dynamic")]
    public class TestDynamicCmdlet : PSCmdlet, IDynamicParameters
    {
        #region FIELDS/CONSTANTS
        private DynamicParameter<int[]> _rp;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = false)]
        public string Name { get; set; }

        #endregion

        #region DYNAMIC
        public object GetDynamicParameters()
        {
            _rp = new DynamicParameter<int[]>("Numbers")
            {
                Mandatory = true
            };
            _rp.ValidatedItems.UnionWith(new int[] { 1, 2, 3, 7 }.Select(x => x.ToString()));

            return new RuntimeDefinedParameterDictionary()
            {
                _rp
            };
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            object num = _rp.GetChosenValue();
            base.WriteObject(num);
        }

        protected override void EndProcessing()
        {

        }

        #endregion

        #region BACKEND METHODS


        #endregion
    }
}