## Release 0.1.0

### New Rules

| Rule ID | Category | Severity | Notes                                                                     |
|---------|----------|----------|---------------------------------------------------------------------------|
| RBI0001 | Usage    | Error    | Binding types should be declared as partial classes.                      |
| RBI0002 | Usage    | Error    | Smart enum values must be tagged with an Field<EnumValue> attribute.      |
| RBI0003 | Usage    | Error    | Smart enum backing field cannot appear more than once.                    |
| RBI0004 | Usage    | Error    | Smart enum backing field must represent a valid C# identifier to be used. |
| RBI0005 | Usage    | Error    | Non Apple framework bindings must provide a library name.                 |
| RBI0006 | Usage    | Warning  | Do not provide the LibraryName for known Apple frameworks.                |
| RBI0007 | Usage    | Error    | Enum values must be tagged with Field<EnumValue>.                         |
