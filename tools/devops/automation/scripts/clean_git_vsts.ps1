param
(
    [Parameter(Mandatory)]
    [String]
    $GithubToken,

    [Parameter(Mandatory)]
    [String]
    $MaccorePath
)

# maccore is special, we use fetch there in some bash scripts, but VSTS uses https.. and some pools don't like the above.. :/
git -C "$MaccorePath" remote remove origin
git -C "$MaccorePath" remote add origin "https://$GithubToken@github.com/xamarin/maccore.git"
git -C "$MaccorePath" remote # don't add -v else we see the pat
