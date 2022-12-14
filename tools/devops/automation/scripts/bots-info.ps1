param (

    [ValidateNotNullOrEmpty ()]
    [string]
    $Token,

    [ValidateNotNullOrEmpty ()]
    [string]
    $PoolName,

    [string]
    $Capability,

    [string]
    $CapabilityValue
)

Import-Module ./MaciosCI.psd1

$vsts =  New-VSTS -Org "devdiv" -Project "DevDiv" -Token $Token
$poolObj = $vsts.Pools.GetPool($PoolName)
$agents = $vsts.Agents.GetAgents($poolObj)

Write-Host "$PoolName bots:"

if ($Capability) {
    # loop over the agents, and if we do have a value, just return the ones with the name, else add the value
    foreach($a in $agents) {
        $value = $a.RestObject.systemCapabilities."$Capability"
        if ($CapabilityValue) {
            if (($value -eq $CapabilityValue)) {
                Write-Host "`tName: $($a.RestObject.name)"
            }
        } else {
            Write-Host "`tName: $($a.RestObject.name); $Capability=$value"
        }
    }
} else {
    foreach($a in $agents) {
        Write-Host "`tName: $($a.RestObject.name)"
    }
}
