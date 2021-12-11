# Test Structure

Testing is an important activity in software development and when done correctly, it allows software development teams to deliver features quickly and with confidence. Well written tests also help document the behavior of an application allowing developers new to the code to quickly learn and experiment with the application. The tests for Scaffold are located in the [Tests](../Tests) directory of this solution.

To run the tests, use the following command;

    dotnet test

This command runs both the unit and integration tests in Scaffold.

## Unit Test

Unit tests are used to test isolated parts of an application. The unit tests in Scaffold are mostly structured according to the following rules.

- For every project in the solution, there should be a corresponding test project with the same project structure.

- For every C# file in a project, there should be a corresponding test file in the corresponding test project.

This structure is important because it allows developers new to the code to quickly identify what parts of the application has been _EXPLICITLY_ tested. Even code coverage does not tell you this!

Nested classes are also heavily used in Scaffold to help organize tests. For an explanation of this, please checkout this article, [_Structuring Unit Tests_](https://haacked.com/archive/2012/01/02/structuring-unit-tests.aspx) by Phil Haack. It also has the side-effect of allowing more tests to be run in parallel in xUnit. See [_Running Tests in Parallel_](https://xunit.net/docs/running-tests-in-parallel).

## Integration Tests

Integrations tests help ensure that different parts of an application work together correctly. In Scaffold, integration tests are done from the point-of-view of a consumer of the application and make heavy use of an [in-memory test server](https://docs.microsoft.com/aspnet/core/test/integration-tests).

When used correctly, an in-memory test server allows individual integration tests to spin up their own instance of your application for full test isolation. Further, each instance can be customized according to the individual needs of each integration test. For example in one of your integration tests, you could test the behavior of your application when the database is down while it is up in the other tests.

Generally to achieve full test isolation in integration tests, external dependencies have to be mocked. This is also the recommended practice in Scaffold so that no external dependencies are required to be spun up before running `dotnet test`.

Testing against real external dependencies should be reserved for a higher order of tests in the test pyramid such as end-to-end tests.

## Code Coverage

Included in this solution is a Cake script with a target that you can use to generate test coverage reports.

    dotnet cake --Target=Test

Once generated, coverage reports are located in the _Artifacts_ directory in a subfolder called _CoverageReports_. Coverage reports are generated for;

- Unit Test
- Integration Tests
- Combined Unit and Integration Tests

### Coverage Thresholds

It is generally accepted that achieving 100% code coverage in an application is unrealistic. It is also not uncommon for testable code to be untested. There is an art in determining the appropriate level of code coverage and using it to guide the software development process. In Scaffold, we can however infer what code coverage should look like given the [hexagonal architecture](./Architecture.md) used.

- Since the application and domain layers do not have any external dependencies (because dependencies go inwards), all code in these layers are testable. 100% code coverage should be expected in these layers.

- Difficult to test or untestable code will be located in each adapter (outermost layer) where interactions with external dependencies happen. In these areas you will not be able to achieve 100% code coverage.

- The total amount of testable code is greater than all the code in the application and domain layers. The maximum possible code coverage for Scaffold will be around here.

## Mutation Testing

Mutation testing is the practice of introducing bugs or mutants into your code to see how good your tests are. Included in each test project in Scaffold is a configuration file for mutation testing with [Styker.NET](https://stryker-mutator.io).

To run a mutation test in Scaffold, navigate to a test project and run;

    dotnet stryker

A mutation report will be produced at end of the test run.
