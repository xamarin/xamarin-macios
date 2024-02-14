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
            # each platform has its version in an environment variable, we need to get it, this
            # could have been inlined when assigning the dictionary but we want to be cleaner.
            $platfformVersionEnvVar = "$($platform.toUpper())_NUGET_OS_VERSION"
            $platformVersion = $val = (Get-Item -Path env:\$platfformVersionEnvVar).Value
            # dictionary with the secrets needed by each matrix
            $platformVars = [ordered]@{
                CLIENT_ID = $Env:API_SCAN_CLIENT_ID;
                TENANT = $Env:API_SCAN_TENANT;
                SECRET = "`$(API_SCAN_SECRET_$($platform.ToUpper()))";
                PLATFORM = $platform.ToUpper();
                VERSION = $platformVersion;
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
