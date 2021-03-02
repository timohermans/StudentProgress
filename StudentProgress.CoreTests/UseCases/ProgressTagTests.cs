using System;
using System.Linq;
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
      var request = new ProgressTagCreateEdit.Command
      {
        Name = "Student gesprek"
      };

      var result = await Execute(async uc => await uc.HandleAsync(request));

      result.IsSuccess.Should().BeTrue();
      var actualTag = Fixture.DataMother.Query<ProgressTag>();
      actualTag.Name.Should().Be((Name)"Student gesprek");
    }

    [Fact]
    public async Task Edits_a_tag()
    {
      var tag = Fixture.DataMother.CreateProgressTag("Studentgesprek");

      var result = await Execute(async uc => await uc.HandleAsync(
        new ProgressTagCreateEdit.Command { Id = tag.Id, Name = "Docentgesprek" })
        );

      result.IsSuccess.Should().BeTrue();
      var assertContext = Fixture.CreateDbContext();
      var actualTags = assertContext.ProgressTags.Where(t => t.Name == "Docentgesprek").ToList();
      actualTags.Count().Should().Be(1);
      actualTags.FirstOrDefault().Name.Should().Be((Name)"Docentgesprek");
    }
  }
}