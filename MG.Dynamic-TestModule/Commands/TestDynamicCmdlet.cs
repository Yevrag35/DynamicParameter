//using MG.Dynamic.Library;
//using MG.Dynamic.Parameter;
using TempDynamic;
using TempDynamic.Extensions;
using TempDynamic.Extensions.Lists;
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
        private static readonly Range[] _ranges = new Range[]
        {
            new Range(0, 45), new Range(45, 549), new Range(7, 10)
        };
        private DynamicParameter<Range, int> _rp;

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = false)]
        public string Name { get; set; }

        #endregion

        #region DYNAMIC
        public object GetDynamicParameters()
        {
            _rp = new DynamicParameter<Range, int>("Numbers", x => x.Difference)
            {
                Mandatory = true,
                ParameterIsArray = false
            };

            _rp.ValidatedItems.AddRange(_ranges);

            return new RuntimeDefinedParameterDictionary
            {
                _rp
            };

            //_rp = new DynamicParameter<int[]>("Numbers")
            //{
            //    Mandatory = true
            //};
            //_rp.ValidatedItems.UnionWith(new int[] { 1, 2, 3, 7 }.Select(x => x.ToString()));

            //return new RuntimeDefinedParameterDictionary()
            //{
            //    _rp
            //};
        }

        #endregion

        #region CMDLET PROCESSING
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
        }

        protected override void ProcessRecord()
        {
            //object num = _rp.GetChosenValue();
            //base.WriteObject(num);
            var range = _rp.GetMatchingSource();
            base.WriteObject(range, true);
        }

        protected override void EndProcessing()
        {

        }

        #endregion

        #region BACKEND METHODS


        #endregion
    }
}