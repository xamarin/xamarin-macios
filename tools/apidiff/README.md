# New Releases

Inside `Make.config` update the `APIDIFF_REFERENCES=` line to point to the `bundle.zip` URL of the currently stable version. E.g.

```
APIDIFF_REFERENCES=https://bosstoragemirror.blob.core.windows.net/wrench/jenkins/d15-9/2dc06c712629feeb179ed112a590d9922caac6e7/53/package/bundle.zip
```

# New Revisions

On the bots each revision rebuilds every assemblies. Each of them will be compared to the downloaded stable version from `APIDIFF_REFERENCES`. Any changes (addition/removal) to the public API will be reported in HTML files.

This can be done manually with `make`. The `.\diff\` directory will contain the diffs in HTML format.

The helper `make merge` target creates a single `api-diff.html` file (from all the `diff\*.html` files) that be used for the documentation web site.
