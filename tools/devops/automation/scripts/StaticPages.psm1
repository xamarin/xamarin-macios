class StaticPages {
    [ValidateNotNullOrEmpty()][string] $UrlPrefix
    [ValidateNotNullOrEmpty()][string] $PrID 
    [ValidateNotNullOrEmpty()][string] $BuildId 

    StaticPages (
        $prefix, 
        $prId,
        $buildId) {
        $this.UrlPrefix = $prefix
        $this.PrID = $prId
        $this.BuildId = $buildId
    }

    [void] WriteComment($stringBuilder) {
        $url = "$($this.UrlPrefix)/pr/PR$($this.PrID)/$($this.BuildId)"
        $stringBuilder.AppendLine("# GitHub pages")
        $stringBuilder.AppendLine()
        $stringBuilder.AppendLine("Results can be found in the following github pages (it might take some time to publish):")
        $stringBuilder.AppendLine()
        $stringBuilder.AppendLine("* [Test results]($url/HtmlReport-sim/tests/vsdrops_index.html)")
        $stringBuilder.AppendLine("* [API diff ]($url/HtmlReport-sim/api-diff/api-diff.html)")
        $stringBuilder.AppendLine("* [API & Generator diff]($url/apicomparison/api-diff.html)")
    }
}
