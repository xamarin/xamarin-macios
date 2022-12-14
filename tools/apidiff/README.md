# New Releases

Inside `Make.config` update the `APIDIFF_REFERENCES=` line to point to the `bundle.zip` URL of the currently stable version. E.g.

The links from our CI will be from `bosstoragemirror.blob.core.windows.net`, but
replace the domain name with `dl.internalx.com`, so the URL looks like this:

```
APIDIFF_REFERENCES=https://dl.internalx.com/wrench/jenkins/d15-9/2dc06c712629feeb179ed112a590d9922caac6e7/53/package/bundle.zip
```

# New Revisions

On the bots each revision rebuilds every assemblies. Each of them will be compared to the downloaded stable version from `APIDIFF_REFERENCES`. Any changes (addition/removal) to the public API will be reported in HTML files.

This can be done manually with `make`. The `.\diff\` directory will contain the diffs in HTML format.

The helper `make merge` target creates a single `api-diff.html` file (from all the `diff\*.html` files) that be used for the documentation web site.

# GitHub token

It's required to provide a [GitHub PAT][GitHubPAT], with scope `read:user` and
`read:org`, in order to download the API reference files. The PAT can be created
[here][CreatePAT].

This can be provided in two ways:

1. Create a file named `~/.config/AUTH_TOKEN_GITHUB_COM`, and add the PAT to
   this file (the file must contain only the PAT, and nothing else). This is the
   recommended way.
2. Export the PAT as the `AUTH_TOKEN_GITHUB_COM` environment variable.

[GitHubPAT]: https://docs.github.com/en/authentication/keeping-your-account-and-data-secure/creating-a-personal-access-token
[CreatePAT]: https://github.com/settings/tokens

