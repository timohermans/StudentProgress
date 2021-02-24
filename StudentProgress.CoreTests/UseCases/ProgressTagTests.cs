using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
  [Collection("db")]
  public class ProgressTagTests : DatabaseTests
  {
    public ProgressTagTests(DatabaseFixture fixture) : base(fixture)
    {
    }

    public async Task<Result> Execute(Func<ProgressTagCreateEdit, Task<Result>> func)
    {
      await using var context = Fixture.CreateDbContext();
      var uc = new ProgressTagCreateEdit(context);
      return await func(uc);
    }

    [Fact]
    public async Task Creates_a_tag()
    {
      await using var ucContext = Fixture.CreateDbContext();

      var result = await Execute(async uc => await uc.HandleAsync(new ProgressTagCreateEdit.Request
      {
        Name = "Student gesprek"
      }));

      result.IsSuccess.Should().BeTrue();
      var actualTag = Fixture.DataMother.Query<ProgressTag>();
      actualTag.Name.Should().Be((Name)"Student gesprek");
    }
  }
}