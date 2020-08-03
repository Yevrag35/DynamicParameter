using MG.Dynamic.Library;
using MG.Dynamic.Parameter;
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
        private DynamicLibrary

        #endregion

        #region PARAMETERS
        [Parameter(Mandatory = false)]
        public string Name { get; set; }

        #endregion

        #region DYNAMIC
        public object GetDynamicParameters()
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