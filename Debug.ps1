[CmdletBinding()]
param ()

$curDir = Split-Path -Parent $MyInvocation.MyCommand.Definition;
Import-Module "$curDir\MG.Dynamic.dll";

$wmiObjs = Get-WmiObject Win32_NetworkAdapter;
$func = [System.Func[wmi,string]] {
	param ($x);
	$x.DeviceId;
};
$global:dp = New-Object MG.Dynamic.DynamicParameter[wmi]("DeviceId", [wmi[]]$wmiObjs, $func, "DeviceId", $true) -Property @{
	Mandatory = $true
	Position = 0
	ValueFromPipeline = $true
};

Function Test-Me()
{
	[CmdletBinding()]
	param ()
	DynamicParam
	{
		$rtDict = New-Object MG.Dynamic.DynamicLibrary;
		$rtDict.Add($global:dp);
		return $rtDict;
	}
	Begin
	{
		$devId = $PSBoundParameters["DeviceId"]
	}
	Process
	{
		$realDevice = $global:dp.GetItemFromChosenValue($devId);
	}
}