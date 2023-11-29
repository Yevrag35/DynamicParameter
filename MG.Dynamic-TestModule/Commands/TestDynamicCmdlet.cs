using MG.Dynamic.Collections;
using MG.Dynamic.Parameters;
using System.Diagnostics;
using System.Management.Automation;

namespace MG.Dynamic.Tests.Module.Commands
{
    [Cmdlet(VerbsDiagnostic.Test, "Dynamic")]
    public sealed class TestDynamicCmdlet : PSCmdlet, IDynamicParameters
    {
        const string ID = "Id";
        static DynamicLibrary _lib = null!;
        static IRuntimeParameter<int> _param = null!;

        [Parameter(Mandatory = true)]
        public string Name { get; set; } = string.Empty;

        public object? GetDynamicParameters()
        {
           if (!this.Name.Equals("TheMan", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            else if (_lib is null)
            {
                _lib = new();
                _param = _lib.Add<int>(ID);
                _param.DefaultParameter.Mandatory = true;
            }

            return (RuntimeDefinedParameterDictionary)_lib;
        }

        protected override void BeginProcessing()
        {
            Debug.Assert(_param is not null);
        }

        protected override void ProcessRecord()
        {
            Debug.Assert(_param is not null);
            if (this.MyInvocation.BoundParameters.TryGetValue(ID, out object? val) && val is int id)
            {
                int boundValue = _param.GetBoundValue();
                Debug.Assert(id == boundValue);
            }
        }
    }
}