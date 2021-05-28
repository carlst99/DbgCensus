echo "Ensure that ReportGenerator is installed: dotnet tool install -g dotnet-reportgenerator-globaltool"\

dotnet test --collect:"XPlat Code Coverage" | Tee-Object -Variable TestOutput

foreach ($string in $TestOutput)
{
	if (Select-String -InputObject $string -Pattern 'coverage.cobertura.xml' -SimpleMatch -Quiet)
	{
		$trimmedString = $string.Trim()
		
		reportgenerator "-reports:$trimmedString" -targetdir:CoverageReport -reporttypes:html
		Invoke-Expression CoverageReport\index.html
		
		break
	}
}