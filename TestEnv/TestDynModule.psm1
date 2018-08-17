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
		$parameter = New-Object Dynamic.Testing.TestParameter
		$atts = @{ Mandatory = $true; Position = 0; ValueFromPipeline = $true };
		$parameter.SetParameterAttributes($atts);
		$parameter.CommitAttributes();
		$library = New-Object Dynamic.Library;
		$library.AddParameter($parameter);
		return $library;
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