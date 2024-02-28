param
(
    [Parameter(Mandatory)]
    [String]
    $SourcesDirectory,

    [Parameter(Mandatory)]
    [String]
    $Platform
)


$downloadPath = Join-Path -Path $SourcesDirectory -ChildPath "not-signed-package"
$destinationPath = Join-Path -Path $SourcesDirectory -ChildPath "APIScan"
New-Item -ItemType Directory -Force -Path $destinationPath
# remove all the files that do not match the platform
$files = Get-ChildItem -Path $downloadPath -Recurse
$filter = "*$Platform*nupkg"
Write-Host "Filter to use is $filter"

foreach ($f in $files) {
  if (-not ($f.Name -like $filter)) {
    Write-Host "Removing file $($f.Name) from $downloadPath"
    Remove-Item -Path $f.FullName
  } else {
    Write-Host "Expanding file $($f.Name) to $destinationPath"
    Expand-Archive -Path $f.FullName -DestinationPath $destinationPath -Force
  }
}
