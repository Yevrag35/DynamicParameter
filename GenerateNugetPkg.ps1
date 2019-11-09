[CmdletBinding(PositionalBinding=$false)]
param
(
	[Parameter(Mandatory)]
	[string] $Configuration,

	[Parameter(Mandatory)]
	[string] $ProjectName,

	[Parameter(Mandatory=$false)]
	[string] $NuspecFile
)

if ($Configuration -eq "Release")
{

	Import-Module "$PSScriptRoot\MG.NuGet.Nuspec.dll" -ErrorAction Stop;
	Set-Location $PSScriptRoot

	if (-not $PSBoundParameters.ContainsKey("NuspecFile"))
	{
		$file = Get-ChildItem -Path $PSScriptRoot -Filter *.nuspec -File | Select-Object -First 1
		if ($null -ne $file)
		{
			$NuspecFile = $file.FullName
		}
	}

	if (-not [string]::IsNullOrEmpty($NuspecFile))
	{
		$editor = New-Object MG.NuGet.Nuspec.NuspecEditor($NuspecFile, "$PSScriptRoot\$ProjectName\Properties\AssemblyInfo.cs")
		$editor.Edit();

		& nuget.exe pack $NuspecFile -properties "Configuration=$($Configuration)"
	}
}