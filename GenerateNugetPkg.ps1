[CmdletBinding(PositionalBinding=$false)]
param
(
	[Parameter(Mandatory)]
	[string] $Configuration,

	[Parameter(Mandatory=$false)]
	[string] $NuspecFile
)

if ($Configuration -eq "Release")
{

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
		[xml]$config = Get-Content -Path $NuspecFile
		$versionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo((Get-ChildItem -Path $config.package.files.file[0].src).FullName).FileVersion
		$config.package.metadata.version = $versionInfo
		$config.Save($NuspecFile)

		& nuget.exe pack $NuspecFile -properties "Configuration=$($Configuration)"
	}

}