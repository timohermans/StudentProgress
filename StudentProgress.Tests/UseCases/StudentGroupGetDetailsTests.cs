using System.Threading.Tasks;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using Xunit;

namespace StudentProgress.Tests.UseCases
{
    public class StudentGroupGetDetailsTests : DatabaseTests
    {
        [Fact]
        public async Task Handle_GroupWithNoStudents_RetrievesSuccessfully()
        {
            await using var dbContext = new ProgressContext(ContextOptions);
            await dbContext.StudentGroup.AddAsync(new StudentGroup("S3 Leon"));
            await dbContext.SaveChangesAsync();
            var useCase = new StudentGroupGetDetails(dbContext);

            var response = await useCase.HandleAsync(new StudentGroupGetDetails.Request(1));

            Assert.Equal(response with {
                Id = 1,
                Mnemonic = null,
                Name = "S3 Leon",
            }, response);
            Assert.Empty(response.Students);
        }
    }
}