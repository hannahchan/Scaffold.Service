# Code Analysis

The [.NET Compiler Platform (_Roslyn_)](https://github.com/dotnet/roslyn) can analyze your code for issues as you type using built-in or third-party analyzers. These analyzers are often called [_Roslyn Analyzers_](https://docs.microsoft.com/dotnet/fundamentals/code-analysis/overview) and are primarily used to help you write more maintainable code. Roslyn Analyzers can be configured by using either an [_EditorConfig_](https://docs.microsoft.com/dotnet/fundamentals/code-analysis/code-style-rule-options) file or a [_rule set_](https://docs.microsoft.com/visualstudio/code-quality/using-rule-sets-to-group-code-analysis-rules) file. Rules in the EditorConfig file override the rules the rule set file.

In Scaffold, the [EditorConfig file](../.editorconfig) is located in the root of the project. The [rule set file](../CodeAnalysis.ruleset) is configured globally for all projects by in a [_Directory.Build.props_](../Directory.Build.props) file.

## Third-party Analyzers

Scaffold uses the following third-party Roslyn Analyzers in some or all of its projects. For an analyzer to work for a given project, it must be installed in that project. To install an analyzer globally for all projects, add the analyzer to [_Directory.Build.props_](../Directory.Build.props).

- [SonarAnalyzer.CSharp](https://github.com/SonarSource/sonar-dotnet) - Helps spot bugs and code smells.
- [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) - Enforce style and consistency when writing C# code.
- [xUnit.Analyzers](https://github.com/xunit/xunit.analyzers) - Helps to write better tests.
