using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;

namespace StudentProgress.Core.UseCases
{
  public class ProgressTagCreateEdit
  {
    private readonly ProgressContext _context;

    public ProgressTagCreateEdit(ProgressContext context)
    {
      _context = context;
    }

    public async Task<Result> HandleAsync(Request request)
    {
      var name = Name.Create(request.Name);

      if (name.IsFailure) {
        return name;
      }

      var tag = new ProgressTag(name.Value);

      await _context.ProgressTags.AddAsync(tag);
      await _context.SaveChangesAsync();

      return Result.Success();
    }

    public record Request
    {
      public string Name { get; set; } = null!;
    }
  }
}