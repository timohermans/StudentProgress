using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
  [Collection("db")]
  public class ProgressGetForCreateOrUpdateTests : DatabaseTests
  {
    public ProgressGetForCreateOrUpdateTests(DatabaseFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Gets_all_data_needed_to_update_progresss()
    {
      var group = Fixture.DataMother.CreateGroup(
         studentNames: new[] { "Timo" },
         milestones: new[] {
                      ("1. Feedback van stakeholders", "Compleetheid documentatie"),
                      ("1. Feedback van stakeholders", "Onderbouwing beslissingen"),
                      ("2. Samenwerking en communicatie", "Samenwerking/communicatie"),
                      ("3. Algorithms", "Circustrein")
             }
         );
      var milestoneCompleteness = group.Milestones.FirstOrDefault(m => m.Artefact == "Compleetheid documentatie");
      var milestoneArgumentation = group.Milestones.FirstOrDefault(m => m.Artefact == "Onderbouwing beslissingen");
      var milestoneCooperation = group.Milestones.FirstOrDefault(m => m.Artefact == "Samenwerking/communicatie");
      var milestoneAlgorithm = group.Milestones.FirstOrDefault(m => m.Artefact == "Circustrein");
      var student = group.Students.FirstOrDefault();
      var progress = Fixture.DataMother.CreateProgressUpdate(
          group, student, 
          feedback: "Or is it?", 
          feedup: "Nope this isn't right", 
          feedforward: null, 
          feeling: Feeling.Bad, 
          date: new DateTime(2021, 1, 7),
          milestoneProgresses: new List<MilestoneProgress>
          {
                    new MilestoneProgress(Rating.Undefined, milestoneCompleteness,"Hij begrijpt het echt nog helemaal niet"),
                    new MilestoneProgress(Rating.Orienting, milestoneArgumentation, null),
                    new MilestoneProgress(Rating.Beginning, milestoneCooperation,"Communicatie is perfect"),
          });
      using var ucContext = Fixture.CreateDbContext();
      var usecase = new ProgressGetForCreateOrUpdate(ucContext);

      var result = await usecase.HandleAsync(new ProgressGetForCreateOrUpdate.Query { GroupId = group.Id, StudentId = student.Id, Id = progress.Id });

      result.IsSuccess.Should().BeTrue();
      var command = result.Value.Command;
      command.Date.Should().Be(new DateTime(2021, 1, 7));
      command.Feedup.Should().Be("Nope this isn't right");
      command.Feedback.Should().Be("Or is it?");
      command.Feedforward.Should().BeNull();
      command.Feeling.Should().Be(Feeling.Bad);
      command.GroupId.Should().Be(group.Id);
      command.Id.Should().Be(progress.Id);
      command.StudentId.Should().Be(student.Id);
      command.Milestones.Should().HaveCount(4);
      command.Milestones.FirstOrDefault(m => m.MilestoneId == milestoneCompleteness.Id)!.Rating.Should().Be(Rating.Undefined);
      command.Milestones.FirstOrDefault(m => m.MilestoneId == milestoneCompleteness.Id)!.Comment.Should().Be("Hij begrijpt het echt nog helemaal niet");
      command.Milestones.FirstOrDefault(m => m.MilestoneId == milestoneArgumentation.Id)!.Rating.Should().Be(Rating.Orienting);
      command.Milestones.FirstOrDefault(m => m.MilestoneId == milestoneArgumentation.Id)!.Comment.Should().BeNull();
      command.Milestones.FirstOrDefault(m => m.MilestoneId == milestoneCooperation.Id)!.Rating.Should().Be(Rating.Beginning);
      command.Milestones.FirstOrDefault(m => m.MilestoneId == milestoneCooperation.Id)!.Comment.Should().Be("Communicatie is perfect");
      command.Milestones.FirstOrDefault(m => m.MilestoneId == milestoneAlgorithm.Id)!.Rating.Should().BeNull();
      command.Milestones.FirstOrDefault(m => m.MilestoneId == milestoneAlgorithm.Id)!.Comment.Should().BeNull();
    }
  }
}
