using module ".\\GitHub.psm1"

class TestResults {
    [string] $ResultsPath # path to the file with the results
    [string] $TestsJobStatus # the value of the env var that lets us know if the tests passed or not can be null or empty
    [string] $Context

    TestResults (
        [string] $path,
        [string] $status,
        [string] $context
    ) {
        $this.ResultsPath = $path
        $this.TestsJobStatus = $status
        $this.Context = $context
    }

    [bool] IsSuccess() {
        return $this.TestsJobStatus -eq "Succeeded"
    }

    [void] WriteComment($stringBuilder) {
        $stringBuilder.AppendLine("## Test results")
        $stringBuilder.AppendLine("")
        if (-not (Test-Path $this.ResultsPath -PathType Leaf)) {
             $stringBuilder.AppendLine(":fire: Tests failed catastrophically on $($this.Context) (no summary found).") 
        } else {
            if ($this.TestsJobStatus -eq "") {
                $stringBuilder.AppendLine(":x: Tests didn't execute on $($this.Context).")
            } elseif ($this.IsSuccess()) {
                $stringBuilder.AppendLine(":white_check_mark: Tests passed on $($this.Context).")
                $stringBuilder.AppendLine("")
                foreach ($line in Get-Content -Path $this.ResultsPath)
                {
                    $stringBuilder.AppendLine($line)
                }
            } else {
                $stringBuilder.AppendLine(":x: Tests failed on $($this.Context)") 
                $stringBuilder.AppendLine("")
                foreach ($line in Get-Content -Path $this.ResultsPath)
                {
                    $stringBuilder.AppendLine($line)
                }
            }
        } 

    }

    [object] GetStatus() {
        $status = $null
        if (-not (Test-Path $this.ResultsPath -PathType Leaf)) {
            $status = [GitHubStatus]::new("failure", "Tests failed catastrophically on $($this.Context) (no summary found).", $this.Context)
        } else {
            if ($this.TestsJobStatus -eq "") {
                $status = [GitHubStatus]::new("error", "Tests didn't execute on $($this.Context).", $this.Context)
            } elseif ($this.IsSuccess()) {
                $status = [GitHubStatus]::new("success", "All tests passed on $($this.Context).", $this.Context)
            } else {
                $status = [GitHubStatus]::new("error", "Tests failed on $($this.Context).", $this.Context)
            }
        } 
        return $status
    }
}
