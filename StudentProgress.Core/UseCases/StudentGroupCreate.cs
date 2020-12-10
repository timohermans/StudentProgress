using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
  public class StudentGroupCreate
  {
    private readonly ProgressContext context;

    public StudentGroupCreate(ProgressContext context)
    {
      this.context = context;
    }

    public record Request
    {
      [Required]
      public string Name { get; init; }

      public string Mnemonic { get; init; }
    }


    public async Task HandleAsync(Request request)
    {
      await context.StudentGroup.AddAsync(new StudentGroup(request.Name));
      await context.SaveChangesAsync();
    }

  }
}