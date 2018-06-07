@{
	GUID = "f70532e7-d043-43ac-a10e-75539c383cdc"
	Author = "Mike Garvey"
	ModuleVersion = "1.0.0"
	PowerShellVersion = "4.0"
	DotNetFrameworkVersion = "4.5.2"
	RootModule = "TestDynModule.psm1"
	RequiredAssemblies = @(
		'Assemblies\DynamicParameter.dll'
	)
	FunctionsToExport = '*'
}