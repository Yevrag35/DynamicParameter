using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;

namespace MG.Dynamic.TestEnv
{
    [Cmdlet(VerbsCommon.Get, "LetsGo")]
    public class LetsGo : PSCmdlet, IDynamicParameters
    {
        #region FIELDS/CONSTANTS


        #endregion

        #region PARAMETERS


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