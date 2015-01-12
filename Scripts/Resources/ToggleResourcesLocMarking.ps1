<#
.SYNOPSIS
    Toggle marking of resource values that helps to identify unlocalized strings.
.DESCRIPTION
	Add a mark to all resource values so that any unlocalized resources can be identified (by the fact that they are unmarked). Or run without mark switch to remove the mark from resource values. Produces a new resources file.
.PARAMETER RdClientLocation
    Path to the root folder of the RdClient solution.
.PARAMETER resourceXmlLocation
    Location of resource file. Shouldn't need to change this.
.PARAMETER resourceXmlFilename
    Name of resource file. Shouldn't need to change this.
.PARAMETER mark
    Use this switch to mark the resources. If this switch is not specified then the script will remove any existing marks.
.EXAMPLE
    ./ToggleResourcesLocMarking.ps1
.EXAMPLE
    ./ToggleResourcesLocMarking.ps1 -mark
.EXAMPLE
    ./CleanupRdClientResourcesFile.ps1 -RdClientLocation C:\repo\rdclient-winrt-universal\RdClient
#>
param(
    [string]$RdClientLocation = "D:\git\rdclient-winrt-universal\RdClient",
    [string]$resourceXmlLocation = "$RdClientLocation\RdClient.Windows\Strings\en-US",
    [string]$resourceXmlFilename = "Resources.resw",
    [switch]$mark
)

[string]$resourceXmlFile = "$resourceXmlLocation\$resourceXmlFilename"
$removeMark = !($Mark)
[string]$locMarking = "(L)"

[xml] $resourcesXml = Get-Content $resourceXmlFile
$dataNodes = $resourcesXml.root.data

foreach ($node in $dataNodes)
{
    [bool]$marked = $node.value.StartsWith($locMarking)
    if ($mark -and !($marked))
    {
        $node.value = [System.String]::Concat($locMarking, $node.value)
    }
    elseif ($removeMark -and $marked)
    {
        $node.value = $node.value.Substring($locMarking.Length)
    }
}

$cleanedResourceFile = "$($resourceXmlFile).new"
Write-Output "writing resource file to $cleanedResourceFile"
$resourcesXml.Save("$cleanedResourceFile")
