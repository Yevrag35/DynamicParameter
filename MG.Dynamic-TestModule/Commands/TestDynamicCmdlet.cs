using MG.Dynamic.Parameters;
using System.Management.Automation;

namespace MG.Dynamic.Tests.Module.Commands;

[Cmdlet(VerbsDiagnostic.Test, "Dynamic")]
public sealed class TestDynamicCmdlet : PSCmdlet, IDynamicParameters
{
	private const string ID = "Id";
	private ValidateSetParameter<int>? _param;

	[Parameter(Mandatory = true, Position = 0)]
	public string Name { get; set; } = string.Empty;

	public object? GetDynamicParameters()
	{
		if (!this.Name.Equals("TheMan", StringComparison.OrdinalIgnoreCase))
		{
			return null;
		}

		if (_param is not null) return _param;

		_param = new()
		{
			Name = ID,
			ParameterType = typeof(int)
		};
		_param.Attributes.DefaultParameter.Mandatory = true;
		_param.Attributes.DefaultParameter.Position = 1;
		_param.TryAddValidValue("14", 14);
		_param.TryAddValidValue("13", 13);

		//return (RuntimeDefinedParameterDictionary)_lib;
		return new RuntimeDefinedParameterDictionary
		{
			{ ID, _param },
		};
	}

	protected override void BeginProcessing()
	{
		//Debug.Assert(_param is not null);
	}

	protected override void ProcessRecord()
	{
		if (_param is not null && _param.TryGetResult(out int pickedNumber))
		{
			this.WriteObject(pickedNumber);
		}
		//Debug.Assert(_param is not null);
		//if (this.MyInvocation.BoundParameters.TryGetValue(ID, out object? val) && val is int id)
		//{
		//	int boundValue = _param.GetBoundValue();
		//	Debug.Assert(id == boundValue);
		//}
	}
}