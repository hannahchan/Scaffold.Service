# StyleCop Analyzers #

StyleCop Analyzers is a code analysis tool that helps enforce style and consistency when writing C# code.

## Adding StyleCop to your Project ##

For each C# project you want code analysis on, you'll need to add StyleCop to that C# project. To add StyleCop, run the following in your C# project directory.

`dotnet add package StyleCop.Analyzers`

By default, StyleCop will apply all StyleCop rules to your project. To control the severity of individual rules, you'll need to reference a rule set file in your `.csproj` file. A configured rule set file has been included in the root directory of this project.

Assuming your C# project is located either in the [Sources](../Sources) or [Tests](../Tests) directories of this project, you can add the included rule set file by including the following under the `<PropertyGroup>` section of your `.csproj` file.

`<CodeAnalysisRuleSet>../../StyleCop.Analyzers.ruleset</CodeAnalysisRuleSet>`

For example,

```
<PropertyGroup>
    <TargetFramework>netcoreapp2.2</TargetFramework>
    <CodeAnalysisRuleSet>../../StyleCop.Analyzers.ruleset</CodeAnalysisRuleSet>
</PropertyGroup>
```

## More Information ##

For more information about StyleCop Analyzers, check out https://github.com/DotNetAnalyzers/StyleCopAnalyzers.
