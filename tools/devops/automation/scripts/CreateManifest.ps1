param(
    [string]
    $ManifestPath = "MaciosCI.psd1"
)

Write-Host "Generating new module manifest."
$modules = Get-ChildItem ./* -Include *.psm1 -Name
New-ModuleManifest -Path $ManifestPath -NestedModules $modules -Guid (New-Guid)  -CompanyName "Microsoft Corp." -ModuleVersion '1.0.0.0' -Description "Module that contains all the scripts needed for the macios ci."  -PowerShellVersion $PSVersionTable.PSVersion.ToString()
