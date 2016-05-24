<####################################################################################
#Date: 		02 December 2015                                                        #
#Author: 	Matthew Rudolph                                                         #
#Script:	Replaces one file with another.                                         #
#Version:	1.0																        #
#####################################################################################
 <#
.SYNOPSIS
	Replaces one file with another.
.DESCRIPTION
	Replaces one file with another.
	Takes two parameters source and target, target does not have to exist but if it does it is deleted and then replaced with the source file.
	Both file names must include the path, either in full or relative to the executing directory.
.EXAMPLE    
		PS C:\Projects\Airy> .\"ReplaceFile.ps1" 'hibernate.cfg.mssql.appveyor.xml' 'hibernate.cfg.mssql.xml'
.NOTES
	Just a short script to reduce the length of the length and complexity of part of the before test Powershell script used for Appveyor.
#>

[CmdletBinding()]
Param(
	[Parameter(Mandatory=$True)]
	[string][alias("s")]
	$source,
	
	[Parameter(Mandatory=$True)]
	[string][alias("t")]
	$target
)

function EntryPoint(){     
	If (Test-Path $target) {
		Remove-Item $target;
	}
  
	Copy-Item $source -Destination $target;
}

EntryPoint;