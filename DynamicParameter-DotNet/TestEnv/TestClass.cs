using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace MG.Dynamic.Testing
{
    [Cmdlet(VerbsDiagnostic.Test, "Command")]
    public class TestCommand : PSCmdlet, IDynamicParameters
    {
        private const string pName = "FileName";

        [Parameter(Mandatory = false)]
        public string TestString { get; set; }

        private IDynamicDefiner _dyn;
        private RuntimeDefinedParameterDictionary dict;

        public object GetDynamicParameters()
        {
            if (dict == null)
            {
                if (_dyn == null)
                {
                    _dyn = new ParameterDefiner(pName, typeof(string[]))
                    {
                        Mandatory = true,
                        Position = 0
                    };
                    _dyn.Aliases.AddRange(new string[2] { "f", "file" });
                    _dyn.AllowEmptyString = true;

                    string curDir = Environment.CurrentDirectory;
                    string[] allFiles = Directory.GetFiles(curDir);
                    _dyn.ValidatedItems.AddRange(allFiles);
                }
                dict = _dyn.NewDictionary();
            }
            return dict;
        }

        protected override void BeginProcessing() => base.BeginProcessing();

        protected override void ProcessRecord()
        {
            string[] chosen = dict[pName].Value as string[];
            Console.WriteLine(chosen);
        }
    }
}
