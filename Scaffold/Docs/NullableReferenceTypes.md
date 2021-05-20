# Nullable Reference Types

To help developers write less buggy code, the nullable aware context has been enabled for all C# projects in Scaffold. This setting `<Nullable>enable</Nullable>`, has been configured in [Directory.Build.props](../Directory.Build.props) and is propagated down to all C# projects.

Learn more about nullable reference types with the links below.

- <https://docs.microsoft.com/dotnet/csharp/language-reference/builtin-types/nullable-reference-types>
- <https://docs.microsoft.com/dotnet/csharp/nullable-references>

## Pitfalls

Nullable reference types are a relatively new feature introduced in C# 8.0. Many libraries are still yet to be annotated with nullable reference types. When working in a nullable context aware application, it is important to keep in mind that;

- A library that an application consumes may not have been annotated with nullable reference types. These libraries do not clearly express whether it is safe to pass in null as an argument or whether a function should return null.

- A library annotated with nullable reference types may still be called by a consumer that is not nullable context aware. If you are writing library code, you may still need to do null reference checks on all your arguments.

## Test Projects

The nullable aware context is currently disabled for all test projects in Scaffold as xUnit 2 does not support nullable reference types, leading to a confusing developer experience. Support is expected in the upcoming xUnit 3.
