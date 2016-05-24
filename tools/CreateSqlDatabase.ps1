<####################################################################################
#Date: 		01 December 2015                                                        #
#Author: 	Matthew Rudolph                                                         #
#Script:	Creates a new empty database on an SQL server.                          #
#Version:	1.0																        #
#####################################################################################
 <#
.SYNOPSIS
    Creates a new empty database on an SQL server.
.DESCRIPTION
    Creates a new empty database on an SQL server.
    Allows the following to be specified.
        Database name
        Primary data file size
        Primary data file growth in percentage
        Log file size
        Log file growth in percentage
.EXAMPLE
    To create a database on a local instance of SQL Server called SQL2014 called TestDatabase:
        PS C:\Projects\Airy> .\"CreateSqlDatabase.ps1" 'LOCALHOST\SQL2014' 'TestDatabase'
.NOTES
    References: http://sqlblog.com/blogs/allen_white/archive/2008/04/28/create-database-from-powershell.aspx
#>

[CmdletBinding()]
Param(
    [Parameter(Mandatory=$True)]
    [string][alias("s")]
    $serverName,
    
    [Parameter(Mandatory=$True)]
    [string][alias("d")]
    $databaseName,

    [Parameter(Mandatory=$False)]
    [double][alias("f")]
    $databaseDataFileSize = "25",

    [Parameter(Mandatory=$False)]
    [double][alias("g")]
    $databaseDataFileGrowth = "25",

    [Parameter(Mandatory=$False)]
    [double][alias("l")]
    $databaseLogFileSize = "10",

    [Parameter(Mandatory=$False)]
    [double][alias("z")]
    $databaseLogFileGrowth = "25"
)

function EntryPoint()
{
    try
    {
        [System.Reflection.Assembly]::LoadWithPartialName("Microsoft.SqlServer.SMO")  | out-null    
        # Configure database.
        $server = new-object -TypeName Microsoft.SqlServer.Management.Smo.Server -argumentlist $serverName;
        $database = new-object -TypeName Microsoft.SqlServer.Management.Smo.Database -argumentlist $server, $databaseName;

        # Configure primary filegroup / data file.
        $primaryFileGroup = new-object -TypeName Microsoft.SqlServer.Management.Smo.FileGroup -argumentlist $database, "PRIMARY";
        $database.FileGroups.Add($primaryFileGroup);
        $primaryFileName = $databaseName;
        $Primaryfile = new-object -TypeName Microsoft.SqlServer.Management.Smo.DataFile -argumentlist $primaryFileGroup, $primaryFileName;
        $primaryFileGroup.Files.Add($Primaryfile);
        $Primaryfile.FileName = $server.Information.MasterDBPath + "\" + $primaryFileName + ".mdf";
        $Primaryfile.Size = [double]($databaseDataFileSize * 1024.0);
        $Primaryfile.GrowthType = "Percent";
        $Primaryfile.Growth = $databaseDataFileGrowth;
        $Primaryfile.IsPrimaryFile = 'True';

        # Configure log file.
        $logName = $databaseName + '_log';
        $logFile = new-object -TypeName Microsoft.SqlServer.Management.Smo.LogFile -argumentlist $database, $logName;
        $database.LogFiles.Add($logFile);
        $logFile.FileName = $server.Information.MasterDBLogPath + '\' + $logName + '.ldf';
        $logFile.Size = [double]($databaseLogFileSize * 1024.0);
        $logFile.GrowthType = 'Percent';
        $logFile.Growth = $databaseLogFileGrowth;

        # Create the database.
        $database.Create();

        Write-Host 'Created database...'
        Write-Host 'Server:           '$serverName;
        Write-Host 'Database:         ' $databaseName;
        Write-Host 'Data file size:   ' $databaseDataFileSize;
        Write-Host 'Data file growth: ' $databaseDataFileGrowth '%';
        Write-Host 'Data log size:    ' $databaseLogFileSize;
        Write-Host 'Data log growth:  ' $databaseLogFileGrowth '%';

        exit 0;
    }
    catch
    {
        throw;
        exit 1;
    }
}

EntryPoint;
