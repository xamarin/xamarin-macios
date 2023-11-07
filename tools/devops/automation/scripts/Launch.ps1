# The pwsh scripts use Write-Debug, but unfortunatly there is not way to set the debug
# level directly from the pwsh yml. What this script does is to check diff enviroment 
# variables and then sets the debug level of pwsh accordingly. This allows to set all 
# scripts to debug mode by simply setting a variable in the pipeline.
if ($Env:SYSTEM_DEBUG -eq "true") {
    $DebugPreference = "Continue"
    Write-Debug "Debug preferences = $DebugPreference"
} else {
    $DebugPreference = "SilentlyContinue"
}
