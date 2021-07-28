# Code Analysis

The [.NET Compiler Platform (_Roslyn_)](https://github.com/dotnet/roslyn) can analyze your code for issues as you type using built-in or third-party analyzers. These analyzers are often called [_Roslyn Analyzers_](https://docs.microsoft.com/dotnet/fundamentals/code-analysis/overview) and are primarily used to help you write more maintainable code. Roslyn Analyzers can be configured by using either an [_EditorConfig_](https://docs.microsoft.com/dotnet/fundamentals/code-analysis/code-style-rule-options) file or a [_rule set_](https://docs.microsoft.com/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules) file. Rules in the EditorConfig file override the rules the rule set file.

Scaffold uses both an EditorConfig file and a rule set file.

## Built-in Analyzers

Starting in .NET 5, the .NET SDK includes analyzers for code styling and quality issues. They do not need to be install separately. Scaffold uses an [EditorConfig file](../.editorconfig) to configure these analyzers.

- Microsoft.CodeAnalysis.Diagnostics - Provides code style analysis.
- [Microsoft.CodeAnalysis.NetAnalyzers](https://github.com/dotnet/roslyn-analyzers) - Provides code quality analysis.

## Third-party Analyzers

Scaffold uses the following third-party Roslyn Analyzers in some or all of its projects. For an analyzer to work for a given project, it must be installed in that project. To install an analyzer globally for all projects, add the analyzer to [_Directory.Build.props_](../Directory.Build.props). Scaffold uses a [rule set file](../CodeAnalysis.ruleset) to configure these analyzers.

- [SonarAnalyzer.CSharp](https://github.com/SonarSource/sonar-dotnet) - Helps spot bugs and code smells.
- [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) - Enforce style and consistency when writing C# code.
- [xUnit.Analyzers](https://github.com/xunit/xunit.analyzers) - Helps to write better tests.

## Rule References

Code Style Rules - <https://docs.microsoft.com/dotnet/fundamentals/code-analysis/style-rules>

Code Quality Rules - <https://docs.microsoft.com/dotnet/fundamentals/code-analysis/quality-rules>

Sonar Static Code Analysis Rules - <https://rules.sonarsource.com/csharp>

StyleCop.Analyzers Rules - <https://github.com/DotNetAnalyzers/StyleCopAnalyzers/blob/master/DOCUMENTATION.md>

xUnit.Analyzers Rules - <https://xunit.net/xunit.analyzers/rules>
