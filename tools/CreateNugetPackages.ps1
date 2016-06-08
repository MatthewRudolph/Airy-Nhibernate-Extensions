<##########################################################################################
#Date: 		04 December 2015                                                              #
#Author: 	Matthew Rudolph                                                               #
#Script:	Builds nuget packages for all projects in a solution that have a nuspec file. #
#Version:	1.0																              #
###########################################################################################
 <#
.SYNOPSIS
	Builds nuget packages for all projects in a solution that have a nuspec file.
.DESCRIPTION
	Builds nuget packages for all projects in a solution that have a nuspec file.
		If no output path is provided then the packages are saved in the bin folder of each project.
.EXAMPLE
	To create release build nuget packages for all projects with a nuspec file in the solution:
		PS C:\Projects\Airy> .\tools\PackageNugetSpecFiles.ps1 'Release'
.NOTES
	
#>

[CmdletBinding()]
Param(
	[Parameter(Mandatory=$False)]
	[string][alias("t")]
	$buildConfiguration = "Release",

	[Parameter(Mandatory=$False)]
	[string][alias("s")]
	$outputPath
)


function PackageNugetSpecFiles()
{
	# Find all the *.nuspec files and deliberately omit NuGet packages.
	$specFiles = @(Get-ChildItem ".\" -Filter "*.nuspec" -Recurse | `
	  ? { $PSItem.FullName -inotmatch "\\packages\\" } | `
	  % { $PSItem.FullName } `
	)

	# Find relevant *.csproj files and call Nuget pack on each of them.
	foreach ($specFile in $specFiles) {
		$folder = $(Split-Path $specFile -Parent)
		$projects = @(Get-ChildItem $folder -Filter "*.csproj" | % { $PSItem.FullName } )
		if ($outputPath -eq ""){
			$outputPath = Join-Path $folder "bin";
		}
		if ($projects.Count -gt 0) {
			$project = @($projects)[0]
		NuGet pack "$project" -Properties "Configuration=$BuildConfiguration" -o "$outputPath"
	  }
	}
}

PackageNugetSpecFiles