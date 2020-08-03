using MG.Dynamic.Parameter;
using MG.Dynamic.Parameter.Generic;
using MG.Dynamic.Library;
using MG.Dynamic.Library.Generic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Management.Automation;

namespace MG.Dynamic.Testing
{
    [Cmdlet(VerbsDiagnostic.Test, "Command")]
    public class TestCommand : PSCmdlet, IDynamicParameters
    {
        private const string pName = "FileName";

        [Parameter(Mandatory = false)]
        public string TestString { get; set; }

        public ComplexDynamicParameter<string, Employee> Employee { get; private set; }


        private RuntimeDefinedParameterDictionary dict;

        public object GetDynamicParameters()
        {
            dict = new RuntimeDefinedParameterDictionary();
            var emp = new Employee
            {
                Id = 72625635,
                Name = "Jim Jones"
            };
            this.Employee = new ComplexDynamicParameter<string, Employee>("Employee")
            {
                Mandatory = true,
                Position = 0,
                ValueFromPipeline = true
            };
            this.Employee.SetDynamicItems(new Employee[1] { emp }, true, x => x.Name);

            dict.Add(this.Employee.Name, this.Employee.AsRuntimeDefinedParameter());
            return dict;
        }

        protected override void BeginProcessing() { }

        protected override void ProcessRecord()
        {
            string[] chosen = dict[this.Employee.Name].Value as string[];
            if (chosen != null)
            {
                IEnumerable<Employee> employees = this.Employee.GetPropertiesFromValues(chosen);
                base.WriteObject(employees, true);
            }
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
