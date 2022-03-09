class Artifacts {
    [object] $Content

    Artifacts (
        [object] $content
    ) {
        $this.Content = $content
    }

    [void] WriteComment($stringBuilder) {
        if ($null -eq $this.Content) {
            $stringBuilder.AppendLine("Artifacts were not provided.")
            return
        }
        if ($this.Content.Count -gt 0) {
            $stringBuilder.AppendLine("# Packages generated")
            $stringBuilder.AppendLine("")
            $stringBuilder.AppendLine("<details><summary>View packages</summary>")
            $stringBuilder.AppendLine("") # no new line results in a bad rendering in the links
            foreach ($a in $this.Content) {
                $url = $a.url
                if ($url.EndsWith(".pkg") -or $url.EndsWith(".nupkg") -or $url.EndsWith(".msi")) {
                    try {
                        $fileName = $a.url.Substring($a.url.LastIndexOf("/") + 1)
                        Write-Debug "Adding link for $fileName"
                        if ($a.url.Contains("notarized")) {
                            $link = "* [$fileName (notarized)]($($a.url))"
                        } else {
                            $link = "* [$fileName]($($a.url))"
                        }
                        $stringBuilder.AppendLine($link)
                    } catch {
                        Write-Debug "Could not get file name for url $url"
                    }
                }
            }
            $stringBuilder.AppendLine("</details>")
        } else {
            $stringBuilder.AppendLine("No packages found.")
        }
    }

    static [Artifacts] FromJsonFiles (
        [object] $contentFile) {
        if ($null -eq $contentFile -or (-not (Test-Path $contentFile -PathType Leaf))) {
            return [Artifacts]::new($null)
        } else {
            $content =  Get-Content $contentFile | ConvertFrom-Json
            return [Artifacts]::new($content)
        }
    }
}
