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
try {
  $user= "AnyUser"
  $base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $user, $VstsToken)))
  $headers = @{Authorization = "Basic {0}" -f $base64AuthInfo}

  # get the url of the build
  $url= $Env:SYSTEM_TEAMFOUNDATIONCOLLECTIONURI + "$Env:SYSTEM_TEAMPROJECT/_apis/build/builds/" + $Env:BUILD_BUILDID + "?api-version=5.1"
  $buildPipeline= Invoke-RestMethod -Uri $url -Headers $headers -Method Get

  Write-host "Build pipeline start time: $($buildPipeline.startTime)"
  $start=[DateTime]::Parse($buildPipeline.startTime).ToString("yyyy-MM-dd HH:mm:ss")

  # hack to get the end time
  $date = Get-Date
  $end=$date.ToString("yyyy-MM-dd HH:mm:ss")
  $start=$date.AddHours(-1).ToString("yyyy-MM-dd HH:mm:ss")

  if (-not $Predicate) {
      Write-Host "Executing: log show --style $Style --start `"$start`" --end `"$end`" > $Output"
      log show --style $Style --start "$start" --end "$end" > $Output
  } else {
      Write-Host "Executing: log show --predicate $Predicate --style $Style --start `"$start`" --end `"$end`" > $Output"
      log show --predicate $Predicate --style $Style --start "$start" --end "$end" > $Output
  }
} catch {
    # Create the output file, because we later try to upload it as an artifact, and *not* uploading
    # if there's *no* file is much harder than just creating the file.
    Write-Host "Exception occurred: $_" | Tee-Object -FilePath $Output -Append
}
