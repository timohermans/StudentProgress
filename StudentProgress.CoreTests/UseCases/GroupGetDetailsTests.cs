using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using Xunit;

namespace StudentProgress.CoreTests.UseCases
{
    [Collection("db")]
    public class GroupGetDetailsTests : DatabaseTests
    {
        public GroupGetDetailsTests(DatabaseFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task Gets_the_most_important_details_per_student()
        {
            var group = Fixture.DataMother.CreateGroup("S3 - Leon", "tips", "Timo", "Ryanne");
            var timo = group.Students.FirstOrDefault(g => g.Name == "Timo");
            Fixture.DataMother.CreateProgressUpdate(group, timo,
                date: new DateTime(2020, 1, 1),
                feedforward: "work on this 1",
                feeling: Feeling.Bad
            );
            Fixture.DataMother.CreateProgressUpdate(group, timo,
                date: new DateTime(2020, 2, 2),
                feedforward: "work on this 2",
                feeling: Feeling.Bad
            );
            var ryanne = group.Students.FirstOrDefault(g => g.Name == "Ryanne");
            Fixture.DataMother.CreateProgressUpdate(group, ryanne,
                date: new DateTime(2020, 5, 5),
                feedforward: "ryanne forward 1",
                feeling: Feeling.Good
            );
            Fixture.DataMother.CreateProgressUpdate(group, ryanne,
                date: new DateTime(2020, 6, 6),
                feedforward: "ryanne forward 2",
                feeling: Feeling.Good
            );
            using var ucConnection = Fixture.CreateDbConnection();
            var useCase = new StudentGroupGetDetails(ucConnection);

            var response = await useCase.HandleAsync(new StudentGroupGetDetails.Request(group.Id));

            response.Should().NotBeNull();
            response!.Id.Should().Be(group.Id);
            response!.Mnemonic.Should().Be("tips");
            response!.Name.Should().Be("S3 - Leon");
            response!.Students.Count().Should().Be(2);
            response!.Students.ElementAt(0).Name.Should().Be("Ryanne");
            response!.Students.ElementAt(1).Name.Should().Be("Timo");
            // TODO: Assert all the things
        }
    }
}