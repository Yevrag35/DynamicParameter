Function Test-DynamicParameter()
{
	[CmdletBinding(PositionalBinding=$false)]
	param
	(
		[parameter(Mandatory=$false, DontShow=$true)]
		[bool] $Whatup = $false
	)
	DynamicParam
	{
		$allItems = Get-ChildItem -Path ".\..\..\GarveyFunctions" -Include * -Recurse
		$global:list = New-Object 'System.Collections.Generic.List[System.IO.FileInfo]'
		foreach ($item in $allItems)
		{
			$global:list.Add($item)
		}
		$names = $global:list.Name
		$atts = @{ Mandatory = $true; Position = 0 }
		$dynParam = New-Object Dynamic.DynamicParameter("File", $names, $atts, @("f", "fils"), $([array]), $true)
		$lib = New-Object Dynamic.DynamicLibrary($dynParam)
		return $lib.Generate()
	}
	Begin
	{
		$chosen = $PSBoundParameters["File"]
		for ($i = $global:list.Count - 1; $i -ge 0; $i--)
		{
			$spare=$false
			$f = $global:list[$i]
			foreach ($n in $chosen)
			{
				if ($f.Name -eq $n)
				{
					$spare = $true
				}
			}
			if (!$spare) { $global:list.Remove($f) > $null }
		}
	}
	Process
	{
		return $global:list
	}
}