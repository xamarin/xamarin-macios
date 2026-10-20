# .NET for Apple Platforms Review Rules

Distilled from past code reviews by senior maintainers of dotnet/macios
(Sebastien Pouliot, Rolf Bjarne Kvinge, Chris Hamons, Manuel de la Peña, Alex Soto),
the conventions in [copilot-instructions.md](../../../copilot-instructions.md),
and the repository's `.editorconfig`.

---

## 0. PRs Excluded from Review

Before reviewing a PR, retrieve its author. PRs authored by `csigs` are generated
by the localization pipeline and must not be reviewed. Stop without reading the
diff, posting inline comments, or submitting a review summary or verdict. Never
rubber-stamp these PRs with a green verdict.

---

## 1. Binding Definition Conventions

Binding definitions (`src/*.cs`) are the core of this repository. They define the
C# API surface for Apple frameworks. Getting them wrong causes broken APIs that
ship to every .NET Apple developer.

| Check | What to look for |
|-------|-----------------|
| **Correct `[Export]` selectors** | The Objective-C selector in `[Export ("...")]` must exactly match Apple's header. A wrong selector causes a 'Selector not found' Objective-C exception at runtime. Verify against Apple's documentation or headers. |
| **`[NullAllowed]` on nullable parameters** | If an Objective-C parameter accepts `nil`, the C# binding must have `[NullAllowed]`. Missing it causes unnecessary `ArgumentNullException`s. Conversely, don't add `[NullAllowed]` if Apple's docs say the parameter must not be nil. Use `[return: NullAllowed]` on return values that can be nil. |
| **Method naming conventions** | C# method names must follow .NET naming guidelines. Methods need a verb: `CreateWatt()` not `Watt()` for factory methods. Properties don't need a verb. In Objective-C there was no distinction between a no-arg method and a read-only property — bind as a property when it represents state, as a method when it performs computation or has side effects. |
| **Delegate/protocol naming** | In protocols of the `*Delegate` kind, use the second parameter for the method name: `DidBeginActivity (HKLiveWorkoutBuilder workoutBuilder, ...)` not `DidBeginActivity (HKWorkoutActivity activity, ...)`. The first parameter is typically the sender. |
| **Use interface types for protocol parameters** | When an Objective-C method takes a protocol type, use the `I`-prefixed interface (e.g., `ICPListTemplateItem[]`) not the concrete class (`CPListItem[]`). This matches Apple's intent and avoids unnecessarily restricting the API. |
| **Properties vs methods** | Objective-C `+unitMethod` that returns a singleton or constant should be bound as a static read-only property (`HKUnit Diopter { get; }`) not a method. Factory methods that take parameters (`+unitWithPrefix:`) should be methods (`CreateWatt (HKMetricPrefix prefix)`). Follow the existing pattern in the same type. |
| **Alphabetical ordering** | Lists of framework entries, enum values, and similar declarations should be alphabetically sorted. |
| **Use custom delegates over `Action<>` for complex callbacks** | When a callback has more than 2-3 parameters, when parameter meaning is unclear (e.g., `Action<bool, NSError>`), or when any parameter or the return type is nullable, define a named delegate type instead. This improves API discoverability and allows proper nullability annotations. |
| **Platform availability attributes** | Every new type and member must have correct platform attributes (`[iOS (version)]`, `[Mac (version)]`, `[TV (version)]`, `[MacCatalyst (version)]`, `[NoiOS]`, `[NoMac]`, `[NoTV]`, `[NoMacCatalyst]`). Removing an attribute makes the API available from the earliest supported version, which is almost certainly wrong. |
| **Don't restrict beyond Apple's documentation** | If Apple documents an API as available on a platform, don't add `[No*]` attributes without good reason. Check headers and documentation carefully. |
| **`[Sealed]` for duplicate selectors** | When two properties share the same Objective-C selector (e.g., during deprecation transitions), use `[Sealed]` on the old one to avoid a compile error. |
| **`ArgumentSemantic` for properties** | Properties that hold references to objects should specify `ArgumentSemantic.Strong`, `.Weak`, `.Copy`, or `.Assign` matching the Objective-C declaration. |

---

## 2. Breaking Changes & Compatibility

| Check | What to look for |
|-------|-----------------|
| **Guard breaking changes with `XAMCORE_5_0`** | When fixing an incorrect binding (wrong parameter type, wrong return type), wrap the old API in `#if !XAMCORE_5_0` with `[Obsolete]` and add the corrected API alongside it. This preserves backward compatibility until the next major breaking-change release. |
| **`[Obsolete]` messages must be helpful** | Obsolete messages should tell users what to use instead: `[Obsolete ("Use '.ctor (ICPListTemplateItem [], string, string)' constructor instead.")]`. |
| **Renaming properties is breaking** | Even `Items` → `Items2` is acceptable as a temporary measure when the correct type must coexist with the old type behind `XAMCORE_5_0`. |
| **Parameter name changes are breaking** | Changing a public method's parameter name (including case) breaks named arguments and late-bound scenarios. |

---

## 3. MSBuild Task Conventions (C#)

Build tasks ship to customers. Getting them wrong causes broken builds.

| Check | What to look for |
|-------|-----------------|
| **`[Required]` properties** | `[Required]` properties must be non-nullable with a default if they're strings or arrays: `public string Foo { get; set; } = "";` or `public ITaskItem[] Bar { get; set; } = [];`. Non-`[Required]` and `[Output]` properties must be nullable (`string?`, `ITaskItem[]?`). |
| **Return `!Log.HasLoggedErrors`** | Task `Execute()` methods should return `!Log.HasLoggedErrors`, not hardcoded `true`/`false`. |
| **SessionId for remote execution** | Tasks that must execute on a Mac (when building from Windows) need the `SessionId` property and `IsMacEnabled` condition. Without these, the task only runs on Windows. |
| **`Inputs`/`Outputs` for incremental builds** | Targets that write files must have `Inputs` and `Outputs` attributes. Missing them breaks incremental builds. |
| **`FileWrites` for intermediate files** | Intermediate files must be added to `@(FileWrites)` so `IncrementalClean` doesn't delete them. Don't use `<Output TaskParameter="..." ItemName="FileWrites" />` — it won't evaluate when the target is skipped. |
| **Condition evaluation timing** | `Condition` on a `<Target>` is evaluated *before* `DependsOnTargets` runs. Comments should document this when it matters. |

**Localizable build messages:** In `msbuild/Xamarin.MacDev.Tasks` and
`tools/assembly-preparer`, flag any user-visible error, warning, informational
message, or exception text that is hard-coded instead of sourced from an English
`.resx` resource (such as `MSBStrings.resx` or `Errors.resx`). See §15 for the rule on
generated translations.

---

## 4. MSBuild Targets & XML

| Check | What to look for |
|-------|-----------------|
| **Underscore prefix for internal names** | Internal targets, properties, and item groups should be prefixed with `_`. |
| **Prefer `$(XDependsOn)` over `BeforeTargets`/`AfterTargets`** | `AfterTargets` runs even if the predecessor failed, causing confusing cascading errors. |
| **Use `->Count()` for empty checks** | `'@(Items->Count())' != '0'` over `'@(Items)' != ''` to avoid huge log messages from string joins. |
| **XML indentation** | MSBuild/XML files use 2 spaces for indentation in `.csproj` files, and tabs for `.props`/`.targets` files (per `.editorconfig`). |

---

## 5. Nullable Reference Types

| Check | What to look for |
|-------|-----------------|
| **`#nullable enable`** | New files should have `#nullable enable` at the top unless already enabled at the project level. |
| **Never use `!` (null-forgiving operator)** | The postfix `!` operator is banned. Add proper null checks or make types non-nullable. AI-generated code frequently sprinkles `!` — reject it. This rule is about postfix `!`, not logical negation `!`. |
| **`ArgumentNullException.ThrowIfNull`** | .NET 9+ code should use `ArgumentNullException.ThrowIfNull (parameter)`. `netstandard2.0` code should use `throw new ArgumentNullException (nameof (parameter))`. |
| **Don't change `string.IsNullOrEmpty()` checks** | If you see an existing `string.IsNullOrEmpty()` check, leave it as-is. |
| **MSBuild task nullable pattern** | See §3 for the `[Required]`/non-`[Required]`/`[Output]` nullable patterns. |

---

## 6. Formatting & Style

C# code formatting is handled automatically and should **not** be reviewed for
formatting issues — doing so just adds noise. Review formatting only for
non-C# text files (XML, shell scripts, YAML, etc.).

| Check | What to look for |
|-------|-----------------|
| **Don't review C# formatting** | C# code is automatically formatted. Do not comment on C# whitespace, brace placement, indentation, or spacing issues. |
| **XML/MSBuild indentation** | `.csproj` files use 2 spaces; `.props`/`.targets` files use tabs (per `.editorconfig`). |
| **Shell scripts** | Should be properly indented and follow existing style in the file. |
| **Max line width ~120 characters** | Applies to non-C# files. Don't merge two lines into a single 160-character monster. |

---

## 7. Async, Cancellation & Thread Safety

| Check | What to look for |
|-------|-----------------|
| **CancellationToken propagation** | Every `async` method that accepts a `CancellationToken` must pass it to ALL downstream async calls. |
| **OperationCanceledException** | Catch-all blocks must NOT swallow `OperationCanceledException`. |
| **Thread safety of shared state** | Static caches and shared mutable state need proper synchronization (`ConcurrentDictionary`, `Interlocked`, or locks). |

---

## 8. Error Handling

| Check | What to look for |
|-------|-----------------|
| **No empty catch blocks** | Every `catch` must capture the `Exception` and log or rethrow it. |
| **Validate parameters** | Throw `ArgumentException` or `ArgumentNullException` for invalid inputs. Use `nameof` for parameter names. |
| **Challenge exception swallowing** | `catch { continue; }` or `catch { return null; }` hides real problems. The default should be to let unexpected exceptions propagate. |
| **Include actionable details in exceptions** | Include the unsupported value or unexpected type. Never throw empty exceptions. |
| **Use `ThrowIf` helpers** | In .NET 9+ projects, prefer `ArgumentOutOfRangeException.ThrowIfNegative`, `ArgumentNullException.ThrowIfNull`, etc. |
| **Initialize output parameters in all paths** | Methods with `out` parameters must initialize them in all error paths. |

---

## 9. Memory Management & Native Interop

| Check | What to look for |
|-------|-----------------|
| **`using` for `IDisposable` resources** | Use `using` statements for objects implementing `IDisposable`, especially `NSObject` subclasses obtained from factory methods. |
| **`GC.KeepAlive` after handle access** | When accessing `.Handle` or `.GetHandle()` on a managed wrapper and passing it to native code, call `GC.KeepAlive (obj)` after the native call to prevent premature GC. Cache the handle in a local before `fixed` blocks. |
| **NativeObject pattern** | `NativeObject` wrappers use an internal `[Preserve]` constructor `(NativeHandle handle, bool owns)` and rely on base `Retain`/`Release` defaults. |
| **Dispose pattern for sealed classes** | A `sealed` class doesn't need `Dispose(bool)` + `GC.SuppressFinalize`. Just implement `IDisposable.Dispose()` directly. |
| **Runtime function naming** | Native runtime functions (`runtime/*.m`) should use the `xamarin_` prefix for release-build functions, not `debug_`. |
| **SIMD type marshalling** | SIMD matrix types like `NMatrix4` use `xamarin_simd__` native shims for marshalling across ObjC boundaries. Don't try to marshal them directly. |

---

## 10. Performance

| Check | What to look for |
|-------|-----------------|
| **Avoid unnecessary allocations** | Don't create intermediate collections when LINQ chaining or a single pass would do. |
| **Pre-allocate collections when size is known** | Use `new List<T> (capacity)` or `new Dictionary<TK, TV> (count)`. |
| **`HashSet.Add()` already handles duplicates** | Don't call `.Contains()` before `.Add()`. |
| **Don't wrap a value in an interpolated string** | `$"{someString}"` is an unnecessary `string.Format` call. |
| **Cache repeated accessor calls** | If `foo.Bar.Baz` is used multiple times, assign to a local. |
| **Watch for O(n²)** | Nested loops, repeated `.Contains()` on a `List<T>`, or LINQ `.Where()` inside a loop. Switch to `HashSet<T>` or `Dictionary<TK, TV>` for lookups. |
| **Extract throw helpers** | `if (x) throw new ...` in hot methods prevents inlining. Extract into a `[DoesNotReturn]` helper. |
| **Place cheap checks before expensive ones** | Test null checks and boolean flags before allocating strings or doing I/O. |
| **Avoid closures in hot paths** | Lambdas that capture locals allocate a closure per call. In loops or frequently-called methods, use static methods or cache delegates. |

---

## 11. Security

| Check | What to look for |
|-------|-----------------|
| **Zip Slip protection** | Archive extraction must validate that every entry path resolves under the destination directory after `Path.GetFullPath()`. |
| **Command injection** | Arguments passed to `Process.Start` must be sanitized. Use `ArgumentList`, not string interpolation. |
| **Path traversal** | `StartsWith()` checks on paths must normalize with `Path.GetFullPath()` first. |

---

## 12. Testing

| Check | What to look for |
|-------|-----------------|
| **Bug fixes need regression tests** | Every PR that fixes a bug should include a test. If the PR says "fixes #1234" but adds no test, ask for one. |
| **Test assertions must be specific** | Avoid `Assert.That (a == b, Is.True)` — it gives poor failure messages. Prefer `Assert.That (a, Is.EqualTo (b))`. One exception: when testing a specific comparison operator. |
| **NUnit conventions** | Use `[TestFixture]`, `[Test]`, `[TestCase]`. Use `Assert.That` with NUnit constraints for rich failure messages. |
| **Platform-specific test guards** | Use `TestRuntime.AssertXcodeVersion()` to skip tests on older OS versions. Scope the guard to the right platform — if a test works on iOS but not macOS, wrap in `#if MONOMAC` not a blanket skip. |
| **Deterministic test data** | Tests should not depend on system locale, timezone, or current date. |
| **Test edge cases** | Empty collections, null inputs, boundary values. If the PR only tests the happy path, suggest edge cases. |
| **Different linking modes** | Consider whether tests should run under "Don't link", "Link SDK only", and "Link all" modes. |

---

## 13. Code Organization

| Check | What to look for |
|-------|-----------------|
| **Use `record` for data types** | Immutable data-carrier types should be `record` or `readonly record struct` — free value equality and boilerplate. |
| **Remove unused code** | Dead methods, speculative helpers, and code "for later" should be removed. No commented-out code — Git has history. |
| **New helpers default to `internal`** | Unless a confirmed external consumer needs them. |
| **Reduce indentation with early returns** | Invert conditions and use `continue` or `return` to reduce nesting. |
| **Don't initialize fields to default values** | `bool flag = false;` is noise. The CLR zero-initializes all fields. |
| **Well-named constants over magic numbers** | `if (retryCount > 3)` should be `if (retryCount > MaxRetries)`. |
| **Use `/// <inheritdoc />`** | For overridden members, prefer `/// <inheritdoc />` over duplicating XML docs. |

---

## 14. Patterns & Conventions

| Check | What to look for |
|-------|-----------------|
| **Use existing utilities** | Check `Runtime`, `TestRuntime`, and other existing utilities before writing new helpers. |
| **Move bash code out of YAML** | Pipeline bash code should be in `.sh` files for syntax highlighting and shellcheck support, not inline YAML blocks. |
| **Unrelated changes** | If a line change seems unrelated to the PR's purpose, ask about it. Keep PRs focused. |
| **Comments explain "why", not "what"** | `// increment i` adds nothing. Explain intent, not mechanics. |
| **Remove stale comments** | If the code changed, update the comment. Wrong comments are worse than no comments. |
| **Track TODOs as issues** | A `// TODO` hidden in code will be forgotten. File an issue and reference it. |
| **`#else`/`#endif` comments** | Annotate `#else` and `#endif` with the original expression when preprocessor blocks are long. |
| **TryCreate pattern** | `TryCreate` methods conventionally return a `bool` + an `out` object/result parameter, and sometimes also an `out` status parameter. |
| **Factory method names** | If `CreateFoo()` sometimes returns an existing instance, rename to `GetOrCreateFoo()`. |
| **Apple feedback tracking** | File a mirror issue for any Apple feedback filed, since others cannot see Apple feedback directly. |

---

## 15. Resource & Localization Files

| Check | What to look for |
|-------|-----------------|
| **Only modify English `.resx` files** | Never modify non-English `.resx` files or `.lcl` files — they are auto-generated by the localization pipeline. |

---

## 16. XML Documentation

| Check | What to look for |
|-------|-----------------|
| **No Xamarin.iOS / Xamarin.Mac references** | Rewrite any mentions of Xamarin.iOS or Xamarin.Mac — these are old product names no longer used. |
| **Remove empty `<para>` and `<remarks>` nodes** | Empty XML doc elements add noise. |
| **Indent nested XML tags** | Use 2 spaces per indentation level within XML doc comments. |
| **Replace include attributes** | If an XML doc has an include attribute, replace it with the inlined content from the included file. |

---

## 17. YAGNI & AI-Specific Pitfalls

These are patterns that AI-generated code consistently gets wrong in this repo:

| Pattern | What to watch for |
|---------|------------------|
| **Reinventing the wheel** | AI creates new infrastructure instead of using existing utilities. ALWAYS check if a similar utility exists. |
| **Over-engineering** | HttpClient injection "for testability", speculative helpers, unused overloads. If no caller needs it today, remove it. |
| **Swallowed errors** | AI catch blocks love to eat exceptions silently. Check EVERY catch block. |
| **Null-forgiving operator** | AI sprinkles `!` everywhere. This is banned in this repo. |
| **Wrong formatting** | AI generates standard C# formatting (no space before parens). This repo uses Mono style with automatic formatting — but AI-generated code in non-C# files (XML, YAML, shell) may have wrong indentation or style. |
| **`string.Empty` and `Array.Empty<T>()`** | AI defaults to these. Use `""` and `[]` instead. |
| **Sloppy structure** | Multiple types in one file, `#region` directives, classes where records would do. New helpers marked `public` when `internal` suffices. |
| **Docs describe intent not reality** | AI doc comments often describe what the code *should* do, not what it *actually* does. |
| **Unused parameters** | AI adds `CancellationToken` parameters but never observes them. |
| **Modifying localization files** | AI modifies non-English `.resx` or `.lcl` files. Only English resource files should be edited. |
| **`git commit --amend`** | Never amend commits. Always create new commits — the maintainer will squash as needed. |
| **Filler words in docs** | "So" at the start of a sentence adds nothing. Be direct. |
