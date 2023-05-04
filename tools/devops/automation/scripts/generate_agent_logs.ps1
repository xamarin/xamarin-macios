param
(

    [Parameter(Mandatory)]
    [String]
    $VstsToken,

    [Parameter(Mandatory)]
    [String]
    $Style,

    [Parameter(Mandatory)]
    [String]
    $Output,

    [String]
    $Predicate
)

# there is no var that gives us the start time, but we can use the rest api to get the time when
# the build started

# User name can be anything. It is the personal access token (PAT) token that matters.
$user= "AnyUser"
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $user, $VstsToken)))
$headers = @{Authorization = "Basic {0}" -f $base64AuthInfo}

# get the url of the build
$url= $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI + "$Env:SYSTEM_TEAMPROJECT/_apis/build/builds/" + $Env:BUILD_BUILDID + "?api-version=5.1"
$buildPipeline= Invoke-RestMethod -Uri $url -Headers $headers -Method Get

$start=[DateTime]::Parse($buildPipeline.startTime).ToString("yyyy-MM-dd HH:mm:ss")

$end=Get-Date -Format "yyyy-MM-dd HH:mm:ss"
Write-Host "end time: $end"

if (-not $Predicate) {
  log show --style $Style --start "$start" --end "$end" > $Output
} else {
  log show --predicate $Predicate --style $Style --start "$start" --end "$end" > $Output
}
