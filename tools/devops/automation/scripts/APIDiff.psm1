class PlatformDiff {
    [ValidateNotNullOrEmpty()] [string] $Platform
    [string] $Html
    [string] $Gist

    PlatformDiff (
        [string] $platform
    ) {
        $this.Platform = $platform
    }

    PlatformDiff (
        [string] $platform,
        [string] $html,
        [string] $gist) {
        $this.Platform = $platform
        $this.Html = $html
        $this.Gist = $gist
    }
}

class APIDiff {
    [string] $Header
    [string] $Result
    [string] $Message
    [Hashtable] $Platforms

    APIDiff (
        [string] $header,
        [Hashtable] $content) {
        # the content object has the following format:
        # 
        #     gist: Hastable with platform as a key and the gist as content
        #     html: Hastable with platform as a key and the html url as content
        #     result: string with the result
        #     message: string with the message
        $this.Header = $header
        $this.Result = $content.result
        $this.Message = $content.message
        $this.Platforms = @{}

        # loop over the gists and create the Platform diff as needed
        if ($null -ne $content.gist -and $null -ne $content.gist.Keys) {
            foreach ($platform in $content.gist.Keys) {
                $diff = $null
                if (-not $this.Platforms.ContainsKey($platform)) {
                    $diff = [PlatformDiff]::new($platform) 
                    $this.Platforms[$platform] = $diff
                } else {
                    $diff = $this.Platforms[$platform]
                }
                $diff.Gist = $content.gist[$platform]
            }
        }
        if ($null -ne $content.html -and $null -ne $content.html.Keys) {
            foreach ($platform in $content.html.Keys) {
                $diff = $null
                if (-not $this.Platforms.ContainsKey($platform)) {
                    $diff = [PlatformDiff]::new($platform) 
                    $this.Platforms[$platform] = $diff
                } else {
                    $diff = $this.Platforms[$platform]
                }
                $diff.Html = $content.html[$platform]
            }
        }
    }
}

class APIDiffComment {
    [object] $FromPR = $null
    [object] $FromStable = $null
    [string] $Generator = $null

    APIDiffComment (
        [object] $prContent,
        [object] $stableContent,
        [string] $generatorContent) {
        $this.FromPR = $null -ne $prContent ? [APIDiff]::new("API Current PR diff", $prContent) : $null
        $this.FromStable = $null -ne $stableContent ? [APIDiff]::new("API diff", $stableContent) : $null
        $this.Generator = $null -ne $generatorContent ? $generatorContent : $null
    }

    static [hashtable] ConvertToHashTable($obj) {
        $result = @{}
        foreach($property in $obj.psobject.properties.name )
        {
            $value = $obj.$property
            if ($value.GetType().Name -eq "PSCustomObject") {
                $value = [APIDiffComment]::ConvertToHashTable($value)
            }
            $result[$property] = $value
        }
       return $result
    }

    static [APIDiffComment] FromJsonFiles (
        [object] $prContentPath,
        [object] $stableContentPath,
        [string] $generatorContent) {
        $prContent = $null
        $stableContent = $null

        if ($null -eq $prContentPath -or (-not (Test-Path $prContentPath -PathType Leaf))) {
            Write-Debug "Path $prContentPath was not found!"
        } else {
            # read the json file, convert it to an object and add a line for each artifact
            $prContent =  Get-Content $prContentPath | ConvertFrom-Json
            # convert custom object to hashtable
            $prContent = [APIDiffComment]::ConvertToHashTable($prContent)
        }

        if ($null -eq $stableContentPath -or (-not (Test-Path $stableContentPath -PathType Leaf))) {
            Write-Debug "Path $stableContentPath was not found!"
        } else {
            # read the json file, convert it to an object and add a line for each artifact
            $stableContent =  Get-Content $stableContentPath | ConvertFrom-Json
            $stableContent = [APIDiffComment]::ConvertToHashTable($stableContent)
        }

        return [APIDiffComment]::new($prContent, $stableContent, $generatorContent)
    }

    [void] WriteDiff($diff, $stringBuilder) {
        # loop over the platforms and write the data
        $stringBuilder.AppendLine("# $($diff.Header)")

        # group the platforms as how the diffs were done
        $commonPlatforms = "iOS", "macOS", "tvOS"
        $legacyPlatforms = @{Title="API diff"; Platforms=@($commonPlatforms + "watchOS");}
        $dotnetPlatforms = @{Title="dotnet API diff"; Platforms=@($commonPlatforms + "MacCatalyst").ForEach({"dotnet-" + $_});}
        $dotnetLegacyPlatforms = @{Title="dotnet legacy API diff"; Platforms=@($commonPlatforms).ForEach({"dotnet-legacy-" + $_});}
        $dotnetMaciOSPlatforms = @{Title="dotnet iOS-MacCatalayst API diff"; Platforms=@("macCatiOS").ForEach({"dotnet-" + $_});}
        $platforms = @($legacyPlatforms, $dotnetPlatforms, $dotnetLegacyPlatforms, $dotnetMaciOSPlatforms)

        foreach ($group in $platforms) {
            $stringBuilder.AppendLine("<details><summary>View $($group.Title)</summary>")
            $stringBuilder.AppendLine("") # no new line results in a bad rendering in the links

            foreach ($platform in $group.Platforms) {
                if ($diff.Platforms.ContainsKey($platform)) {
                    $platformDiff = $diff.Platforms[$platform]
                    $htmlLink  = ""
                    $gistLink = ""
                    if ($null -ne $platformDiff.Html) {
                        $htmlLink = "[vsdrops]($($platformDiff.Html))"
                    }
                    if ($null -ne $platformDiff.Gist) {
                        $gistLink = "[gist]($($platformDiff.Gist))"
                    }
                    $line = @("*", $platform, $htmlLink, $gistLink) -join " "
                    $stringBuilder.AppendLine($line)
                } else {
                    $stringBuilder.AppendLine("* :fire: $platform :fire: Missing files")
                }
            }
            $StringBuilder.AppendLine("</details>")
            $StringBuilder.AppendLine("")
        }
        $StringBuilder.AppendLine("")
    }

    [void] WriteComment($stringBuilder) {
        if ($null -eq $this.FromPR) {
            $stringBuilder.AppendLine("* :warning: API diff urls have not been provided.")
        } else {
            $this.WriteDiff($this.FromPR, $stringBuilder)
        }

        if ($null -eq $this.FromStable) {
            $stringBuilder.AppendLine("* :warning: API Current PR diff urls have not been provided.")
        } else {
            $this.WriteDiff($this.FromStable, $stringBuilder)
        }

        # the generator content is a file which we simply read
        if ($null -eq $this.Generator) {
            $stringBuilder.AppendLine("* :warning: Generator diff comments have not been provided.")
        } else {
            if (Test-Path $this.Generator -PathType Leaf) {
                $stringBuilder.AppendLine("# Generator diff")
                $stringBuilder.AppendLine("")
                # ugly workaround to get decent new lines
                foreach ($line in Get-Content -Path $this.Generator)
                {
                    $stringBuilder.AppendLine($line)
                }
                $stringBuilder.AppendLine("")
            } else {
                $stringBuilder.AppendLine("* :warning: Path $($this.Generator) was not found!")
            }
        }
    }
}

<# 
    .SYNOPSIS
        Creates a new APIDiffComment object from the data present in the given hash table.
#>
function New-APIDiffComment {
    param (
        [object]
        $PRContent,
        [object]
        $StableContent,
        [string]
        $GeneratorContent
    )
    return [APIDiffComment]::new($PRContent, $StableContent, $GeneratorContent)
}


<# 
    .SYNOPSIS
        Creates a new APIDiffComment object from the data present in a json file. 
#>
function New-APIDiffCommentFromFiles {
    param (
        [string]
        $PRContentPath,
        [string]
        $StableContentPath,
        [string]
        $GeneratorContent
    )
    return [APIDiffComment]::FromJsonFiles($PRContentPath, $StableContentPath, $GeneratorContent)
}

Export-ModuleMember -Function New-APIDiffComment
Export-ModuleMember -Function New-APIDiffCommentFromFiles
