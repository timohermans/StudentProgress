# Student Progress

[![Build Status](http://83.81.134.165:8080/api/badges/timohermans/StudentProgress/status.svg)](http://83.81.134.165:8080/timohermans/StudentProgress)

Project to track people, whether that's students, team members or yourself.

## Feature showcase

![showcase](./features.gif)

## Installation
To run the project, execute the following command in this folder:

```bash
docker-compose up -d
```
The app will be available on `http://localhost:80`. The app and database will keep running **until you stop with `docker-compose down` or using the GUI**


To build new changes into the running docker image, execute the following command:
```bash
docker-compose up -d --build
```

## Config

Project works out of the box. You can, however, change a couple of variables

> Change these values in the [docker-compose.yml](docker-compose.yml) file if necessary

### Port

By default the port is 80. See `ports` in the `student-progress` service

### Database

By default the docker-compose spins up a PostgreSQL instance.
You can use your own PostgreSQL instance by adding and/or changing the following environment variables on the `student-progress` service:

> Note that you cannot use a different database provider for now, because I've used Dapper for several queries

- DB_HOST
- DB_PORT
- DB_USERNAME
- DB_PASSWORD
- DB_DATABASE

## Development

### Migrations

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

### Integration tests

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

### Unit Tests

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

### Webpack integration

Used this approach, which is awesome: [use npm with .net core mvc](https://medium.com/@lars.willemsens/managing-asp-net-core-mvc-front-end-dependencies-with-npm-and-webpack-part-1-1f6a8b8a1289)