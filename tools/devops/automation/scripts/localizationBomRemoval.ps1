function RemoveUtf8Bom
{
    param(
        [string]$FilePath
    )

    $encoding = New-Object -TypeName System.Text.UTF8Encoding
    $content = Get-Content -Path $FilePath -Raw
    [System.IO.File]::WriteAllLines($FilePath, $content, $encoding)
}

foreach ($locFile in (Get-Content -Path $env:XLocFileList))
{
    RemoveUtf8Bom -FilePath $locFile
}