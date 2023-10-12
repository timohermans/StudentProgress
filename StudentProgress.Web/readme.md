# Student Progress Core

## Resources

- [Result class usage](https://josef.codes/my-take-on-the-result-class-in-c-sharp/)
- [Entity Framework docs](https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli)
- [ASP.NET core documentation](https://learn.microsoft.com/en-us/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-7.0)

## Architecture migration notes

Starting from now (2023-10-12), it should be encouraged to remove all traces of `Core` and `ProgressContext` from the
system and their tests.
This means that some boyscouting is to be done.
When working on a page:

- start actively working on removing `ProgressContext`,
- add to `WebContext`
- add `htmx` where possible to make it fancy
- (optional) start including more Bootstrap 5.3 (especially with css variables)

## TODO

- Change import tool to new state

## Migrations

To apply migrations:

```bash
dotnet ef database update --startup-project "./StudentProgress.Web/StudentProgress.Web.csproj" --project="./StudentProgress.Core/StudentProgress.Core.csproj"
```

To add migrations:

```bash
dotnet ef migrations add InitialCreate --startup-project "./StudentProgress.Web/StudentProgress.Web.csproj" --project="./StudentProgress.Core/StudentProgress.Core.csproj"
```

To remove migrations:

```bash
dotnet ef migrations remove --startup-project "./StudentProgress.Web/StudentProgress.Web.csproj" --project="./StudentProgress.Core/StudentProgress.Core.csproj"
```

## Integration tests

Integration tests are done through a real database.
Please be advised with using integration tests:

- Only create 2 tests for a usecase:
    - One for the longest happy path
    - One failure test. Preferrably the longest, but any will do.
- One exception to this rule is when there are a lot of database specific constraints, like db uniqueness

Creating a new integration test file is easy:

- Add a new class
- Add the attribute `[Collection("db")]` above the class definition
- Implement abstract class `DatabaseTests`
- You will have to add the constructor: `public <ClassName>(DatabaseFixture fixture) : base(fixture) {}`
- You now have access to the `Fixture` property

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