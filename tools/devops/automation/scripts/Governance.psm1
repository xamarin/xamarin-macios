class APIScanConfiguration {
    [string[]] $enabledPlatforms

    APIScanConfiguration (
        [string[]] $enabledPlatforms) {
        $this.enabledPlatforms = $enabledPlatforms
    }

    [string] Create() {
        $vars = [ordered]@{}
        Write-Host "enabledPlatforms: $($this.enabledPlatforms)"

        foreach ($platform in $this.enabledPlatforms) {
            # dictionary with the secrets needed by each matrix
            $platformVars = [ordered]@{
                CLIENT_ID = $Env:API_SCAN_CLIENT_ID;
                TENANT = $Env:API_SCAN_TENANT;
                SECRET = "`$(API_SCAN_SECRET_$($platform.ToUpper()))";
                PLATFORM = $platform.ToUpper();
            }
            $vars[$platform] = $platformVars
        }

        return $vars | ConvertTo-Json
    }

}

function Get-APIScanConfiguration {
    param
    (
        [Parameter(Mandatory)]
        [string]
        [AllowEmptyString()]
        $EnabledPlatforms
    )

    $arrEnabledPlatforms = -split $EnabledPlatforms
    $config = [APIScanConfiguration]::new($arrEnabledPlatforms)
    return $config.Create()
}

# export public functions, other functions are private and should not be used outside the module.
Export-ModuleMember -Function Get-APIScanConfiguration
