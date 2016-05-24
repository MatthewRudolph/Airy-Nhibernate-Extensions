<##########################################################################################
#Date: 		03 December 2015                                                              #
#Author: 	Matthew Rudolph                                                               #
#Script:	Sets the version of AssemblyInfo.cs and *.nuspec files.                       #
#Version:	1.0																              #
###########################################################################################
 <#
.SYNOPSIS
	Sets the version of AssemblyInfo.cs and *.nuspec files.
.DESCRIPTION
	Sets the version of AssemblyInfo.cs and *.nuspec files.
		Uses the semantic version system to set versions.
		Pass the version in the following format major.minor.patch.build.
		If releaseType is Release (default) then no text is appended to the nuget version.
		If releaseType is not Release then it's value is suffixed with the build number and appended to the patch version.
			AssemblyVersion = major.minor.patch.0  (i.e. the last part will always be 0)
			AssemblyFileVersion = major.minor.patch.build
			AssemblyInformationalVersion = major.minor.patch.-releaseType+build
			NugetVersion = major.minor.patch(-releaseType+)build
.EXAMPLE
	To create release build nuget packages for all projects with a nuspec file in the solution:
		PS C:\Projects\Airy> .\tools\SetVersion.ps1
		PS C:\Projects\Airy> .\tools\SetVersion.ps1 'Release'
		PS C:\Projects\Airy> .\tools\SetVersion.ps1 'Alpha'
.NOTES
	
#>
[CmdletBinding()]
Param(
	[Parameter(Mandatory=$True)]
	[string][alias("s")]
	$version,
	
	[Parameter(Mandatory=$False)]
	[string][alias("t")]
	$releaseType = "Release"
)

function CalculateVersions()
{
	#Regex to match exactly 4 part version:
	$hasVersion = "'"+$version+"'" -match '([0-9]+)\.([0-9]+)\.([0-9]+)\.([0-9]+)';

	if (!$hasVersion)
	{
		Write-Output "No version found, using 0.1.0.1000 instead.";
		$major=0;
		$minor=1;
		$patch=0;
		$build=1000;
	}
	else
	{
		$versionFormattedCorrectly = $matches[0] -match "(?<major>[0-9]+)\.(?<minor>[0-9]+)\.(?<patch>[0-9]+)\.(?<build>[0-9]+)";
		if (!$versionFormattedCorrectly)
		{
			Write-Output "The supplied version is not formatted correctly: " $version;
			exit 1;
		}
	
		$major=$matches['major'] -as [int];
		$minor=$matches['minor'] -as [int];
		$patch=$matches['patch'] -as [int];
		$build=$matches['build'] -as [int];
	}

	$Script:AssemblyVersion = "$major.$minor.$patch.0";
	$Script:AssemblyFileVersion = "$major.$minor.$patch.$build";
	$Script:AssemblyInformationalVersion = "$major.$minor.$patch-$releaseType$build";
	$Script:NugetVersion = "$major.$minor.$patch";
	if ($releaseType -notmatch "^Release$")
	{
		$Script:NugetVersion = "$major.$minor.$patch-$releaseType$build";
	}

	Write-Output "Using Assembly Version: $AssemblyVersion";
	Write-Output "Using File Version: $AssemblyFileVersion";
	Write-Output "Using Informational Version: $AssemblyInformationalVersion";
	Write-Output "Using Nuget Version: $nugetVersion";
}

function SetAssemblyVersion()
{	
	$fileAssemblyVersion = 'AssemblyVersion("' + $AssemblyVersion + '")';
	$fileFileVersion = 'AssemblyFileVersion("' + $AssemblyFileVersion + '")';
	$fileInformationalVersion = 'AssemblyInformationalVersion("' + $AssemblyInformationalVersion + '")';

	$foundFiles = get-childitem .\*AssemblyInfo.cs -recurse
	foreach( $file in $foundFiles )
	{
		$content = Get-Content "$file";
		Write-Output "Patching $file";
		$content -replace 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $fileAssemblyVersion `
				 -replace 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)', $fileFileVersion `
				 -replace 'AssemblyInformationalVersion\("[0-9]+(\.([0-9]+|\*)){1,3}.*"\)', $fileInformationalVersion | Set-Content "$file";
		Write-Output "Completed patching $file";
	}
}

function SetNugetVersion()
{
	$nugetFoundFiles = get-childitem .\*.nuspec -recurse
	foreach( $nugetSpecFile in $nugetFoundFiles )
	{
		$nugetSpecContent = [xml](Get-Content (Resolve-Path $nugetSpecFile));		
		Write-Output "Patching $nugetSpecFile";
		$nugetSpecContent.package.metadata.version = $NugetVersion;
		$nugetSpecContent.Save((Resolve-Path $nugetSpecFile));
		Write-Output "Completed patching $nugetSpecFile";
	}
}

CalculateVersions;
SetAssemblyVersion;
SetNugetVersion
Exit 0;
