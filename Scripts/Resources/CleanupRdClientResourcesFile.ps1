<#
.SYNOPSIS
    Generate a new resources file containing only resources that are referenced
.DESCRIPTION
	Loads the Resources.resw xml file, and then searches through all the files in the RdClient solution for references to each resource defined in the resources file. It then creates a new resources file containing only the references that are referenced in the solution.
.PARAMETER RdClientLocation
    Path to the root folder of the RdClient solution.
.PARAMETER resourceXmlLocation
    Location of resource file. Shouldn't need to change this.
.PARAMETER resourceXmlFilename
    Name of resource file. Shouldn't need to change this.
.EXAMPLE
    ./CleanupRdClientResourcesFile.ps1
.EXAMPLE
    ./CleanupRdClientResourcesFile.ps1 -RdClientLocation C:\repo\rdclient-winrt-universal\RdClient
#>

param(
    [string]$RdClientLocation = "D:\git\rdclient-winrt-universal\RdClient",
    [string]$resourceXmlLocation = "$RdClientLocation\RdClient.Windows\Strings\en-US",
    [string]$resourceXmlFilename = "Resources.resw"
)

[string]$resourceXmlFile = "$resourceXmlLocation\$resourceXmlFilename"
[xml] $resourcesXml = Get-Content $resourceXmlFile
$dataNodes = $resourcesXml.root.data

$files = @(get-childitem $RdClientLocation -Recurse | 
            ? { !($_.PSiscontainer) -and !($_.FullName -like "*$resourceXmlFilename*" -or $_.FullName -like "*bin\*" -or $_.FullName -like "*obj\*") })

$unusedNodes = @()
$usedNodes = @()
$possiblyUnusedNodes = @()
foreach ($node in $dataNodes)
{
    $resourceName = $node.name
    if ($resourceName.EndsWith("_String"))
    {
		$possiblyUnusedNodes += $node
		Write-Output "$resourceName Needs manual investigation - may be used by TypeToLocalizedStringConverter"
    }
    else
    {
		#Ignore tooltip, we just want the Uid
		$toolTipIndex = $resourceName.LastIndexOf("ToolTipService.ToolTip")
		if ($toolTipIndex -gt 0)
		{
			$resourceName = $resourceName.Substring(0, $toolTipIndex)
		}
		#Ignore property, we just want the Uid
		$lastDotIndex = $resourceName.LastIndexOf(".")
		if ($lastDotIndex -gt 0)
		{
			$resourceName = $resourceName.Substring(0, $lastDotIndex)
		}
		Write-Output "$resourceName"
    
		$results = @( $files | Select-String -SimpleMatch "$resourceName")
		if ($results.Count -le 0)
		{
			$unusedNodes += $node
			Write-Output "No match found!"    
		}
		else
		{
			$usedNodes += $node
			Write-Output $results	
		}
	}	
	Write-Output "."
}

Write-Output "Removing unused resources..."
foreach($node in $unusedNodes)
{
    Write-Output "$($node.name)"
    $resourcesXml.root.RemoveChild($node) | Out-Null
}

Write-OUtput "Resources that are not referenced directly but may be used in TypeToLocalizedStringConverter..."
foreach($node in $possiblyUnusedNodes)
{
    Write-Output "$($node.name)"
}

Write-Output "$($dataNodes.Count) total resources"
Write-Output "$($usedNodes.Count) used resources"
Write-Output "$($unusedNodes.Count) unused resources" 
Write-Output "$($possiblyUnusedNodes.Count) possibly unused resources"

$cleanedResourceFile = "$($resourceXmlFile).new"
Write-Output "writing cleaned resource file to $cleanedResourceFile"
$resourcesXml.Save("$cleanedResourceFile")


