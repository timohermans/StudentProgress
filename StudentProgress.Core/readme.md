# Student Progress Core

## Migrations

Make sure that the command are run in the Core project

```bash
cd StudentProgress.Core
```

To add migrations:

```bash
dotnet ef migrations add InitialCreate --startup-project "../StudentProgress.Web/StudentProgress.Web.csproj"
```

To remove migrations:

```bash
dotnet ef migrations remove --startup-project "../StudentProgress.Web/StudentProgress.Web.csproj"
```

## Integration tests

Integration tests are done with a real database.
Please be advised with using integration tests:

- Only create 2 tests for a usecase:
  - One for the longest happy path
  - One failure test. Preferrably the longest, but any will do.
- One exception to this rule is when there are a lot of database specific constraints, like db uniqueness

## Unit Tests

Test are written using [xUnit](https://xunit.net/docs/shared-context)

Assertions are done with [Fluent assertions](https://fluentassertions.com/)

These combined, a test looks like this:

```csharp
[Fact]
public void Name_cannot_be_empty() {
    var name = Name.Create("");
    
    name.IsSuccess.Should().BeFalse();
} 
```