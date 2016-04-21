# pmcs: preprocessing frontend to mcs

pmcs performs search and replace operations on source code before invoking
the actual mcs C# compiler. Original input files are not modified by default;
rather, temporary files are passed to the compiler containing the preprocessed
source code.

Search and replace operations are not performed for the entire contents
of a file. Instead, the preprocessor is aware of C# comments and string
literals. It will only perform replacements on "tokens" in C# code, where a
"token" is a group of characters being letters, digits, or one of `_`, `.`,
or `:`.

## Command Line Options ##

| Option                  | Description |
| ----------------------- | ----------- |
| `-h`, `-help`           | Show help |
| `-v`, `-verbose`        | Be verbose with output |
| `-compiler:VALUE`       | Set the mcs compiler executable to invoke to `VALUE` |
| `-P`, `-profile:VALUE`  | Load a pmcs XML profile from `VALUE`; see [XML Profiles](#xml-profiles) |
| `-ignore:VALUE`         | Do not preprocess `VALUE` |
| `-global-replace:VALUE` | Define a global replacement where `VALUE` = `PATTERN=REPLACEMENT`; see [Replacement Definitions on the Command Line](#replacement-definitions-on-the-command-line) |
| `-enum-replace:VALUE`   | Define an enum backing type replacement where `VALUE` = `PATTERN=REPLACEMENT`; see [Replacement Definitions on the Command Line](#replacement-definitions-on-the-command-line) |
| `-target:VALUE`         | specifies the format of the output assembly |
| `-omit-line-directive`  | Do not generate a C# `#line` directive at the start of preprocessed files |
| `-skip-mdb-rebase`      | do not rebase path names in .mdb symbol files (if -debug was specified) |
| `-skip-compile`         | Do not perform compilation (preprocess only) |
| `-keep`                 | Do not delete preprocessed intermediate files (implied with `-in-place`) |
| `-in-place`             | Preprocess in place instead of outputting intermediate files. **WARNING:** this can be dangerous -- only use this option if your sources are under version control and do not have local changes or are otherwise backed up. |
| `-stdout`               | Write preprocessed output to standard output (implies `-skip-compile` and no files on disk are modified) |

## MDB Rebasing ##

The paths that pmcs passes to the actual compiler will have a different name
from what was passed to pmcs itself - these files will contain the actual
preprocessed output. For instance:

	$ pmcs -profile:foo Bar.cs

Here, `Bar.cs` is the original source which needs to be preprocessed. By
default (e.g. `-in-place` is not specified), pmcs will generate a file
named `~.pmcs-foo.Bar.cs` which will be passed to the actual compiler.

When the actual compiler  compiles these files, their preprocessed names will
end up in the `.mdb` symbol files, which is undesirable. By default pmcs will
rebase the symbol files with the original names passed to pmcs.

This can be disabled by passing `-skip-mdb-rebase` to pmcs.

## Replacements ##

Three replacement kinds are supported:

| Kind   | Description                                                          |
| ------ | -------------------------------------------------------------------- |
| Exact  | Only tokens matching exactly are replaced                            |
| Prefix | Tokens starting with a pattern have the matching prefix replaced     |
| Regex  | Tokens matching a regular expression have the matching part replaced |

Additionally, there are two replacement token scopes:

| Scope  | Description                                                               |
| ------ | ------------------------------------------------------------------------- |
| Global | Perform replacements on any kind of token                                 |
| Enum   | Perform replacements only on "backing type" tokens for an enum definition |

The "enum" token replacement scope is useful for defining an alternate
replacement value for enum backing types when a global replacement value
is already defined for the same pattern:

| Pattern | Global Replacement   | Enum Replacement |
| ------- | -------------------- | ---------------- |
| nint    | global::System.Int32 | int              |

The enum replacement scope has priority over the global scope.

### Replacement Definitions on the Command Line ###

Replacements can be defined on the command line by specifying one of the
two replacement options:

* `-global-replace:PATTERN=REPLACEMENT`
* `-enum-replace:PATTERN=REPLACEMENT`

Where `PATTERN` is the pattern to be matched against a token, and `REPLACEMENT`
is the new value for the matching token or part of the matching token.

To define a Regex replacement on the command line, prefix `PATTERN` with `/`:

* `-global-replace:"/^(global::System\.|System\.)?nint=global::System.Int32"`

To define a Prefix replacement on the command line, prefix `PATTERN` with `^`:

* `-global-replace:"^MonoTouch=MonoMac"`

An exact replacement does not need a prefix to `PATTERN`:

* `-enum-replace:nint=int`

### Replacement Definitions in an XML Profile ###

Replacements can also be defined inside a pmcs XML profile with the
`<replacement>` element. Each of these elements must have a `scope`
attribute with a value of either `global` or `enum` and can contain
child elements named either `exact`, `prefix`, or `regex` with attributes
`pattern` and `replacement`.

An XML version of the command line replacement definitions above:

```xml
<replacements scope="global">
  <regex
    pattern="^(global::System\.|System\.)?nint"
    replacement="global::System.Int32"/>
  <prefix pattern="MonoTouch" replacement="MonoMac"/>
</replacements>
<replacements scope="enum">
  <exact pattern="nint" replacement="int"/>
</replacements>
```

### Replacement Examples Overview ###

In the above examples, tokens that are equal to `global::System.nint`,
`System.nint`, or `nint` (as matched by the regular expression replacement)
are replaced with `global::System.Int32`, effectively translating a fake
`nint` type into a real `System.Int32` type.

Because enums can only be backed by certain C# keywords (not types), an
enum-scoped replacement is defined to translate `nint` to `int`, the C#
keyword for `System.Int32`.

Finally, a prefix replacement is used for translating the `MonoTouch.*`
namespaces into `MonoMac.*` namespaces. Prefix replacements are faster
than regular expression replacements.

## XML Profiles ##

XML Profiles provide an alternate means to specifying the compiler
executable, extra compiler options, and replacement definitions. Each
profile must start with a `<pmcs>` root element and can contain any
number of the following child elements:

### Ignore Paths ###

`<ignore>VALUE</ignore>` element: do not preprocess `VALUE`

### Compiler Configuration ###

`<compiler>` container element. No attributes.

##### Child elements: #####

* `<executable>VALUE</executable>`: specify the compiler
executable to be `VALUE`
* `<option>VALUE</option>`: append `VALUE` as a command line option to
the compiler

### Replacements ###

`<replacements scope="global|enum">` container element with `scope`
attribute; `scope` should be either `global` or `enum`.

See [Replacement Definitions in an XML Profile](#replacement-definitions-in-an-xml-profile)
for more detail.

##### Child elements: #####

* `<exact pattern="PATTERN" replacement="REPLACEMENT" />`
* `<prefix pattern="PATTERN" replacement="REPLACEMENT" />`
* `<regex pattern="PATTERN" replacement="REPLACEMENT" />`

The child elements specify which [kind of replacement to
perform](#replacement-definitions-in-an-xml-profile).

### Includes ###

`<include>PATH</include>` inline another pmcs profile in `<include>` order.
where `PATH` is relative to the parent directory of the current profile.

### Example ###

```xml
<pmcs>
  <compiler>
    <executable>/usr/local/bin/mcs</executable>
    <option>-define:ARCH_32</option>
    <option>-sdk:4</option>
  </compiler>
  <ignore>Compat.cs</ignore>
  <replacements scope="global">
    <exact pattern="CGSize" replacement="SizeF" />
    <exact pattern="CGPoint" replacement="PointF" />
    <exact pattern="CGRect" replacement="RectangleF" />
	<prefix pattern="MonoTouch" replacement="MonoMac" />
    <regex pattern="^(global::System\.|System\.)?nfloat" replacement="global::System.Single" />
    <regex pattern="^(global::System\.|System\.)?nint" replacement="global::System.Int32" />
    <regex pattern="^(global::System\.|System\.)?nuint" replacement="global::System.UInt32" />
  </replacements>
  <replacements scope="enum">
    <exact pattern="nint" replacement="int" />
    <exact pattern="nuint" replacement="uint" />
  </replacements>
</pmcs>
```
