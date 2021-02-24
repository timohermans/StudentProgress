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

    public async Task<Result> HandleAsync(Command request)
    {
      var name = Name.Create(request.Name);

      if (name.IsFailure)
      {
        return name;
      }

      if (request.Id.HasValue)
      {
        await UpdateTagWith(request.Id.Value, name.Value);
      }
      else
      {
        await CreateNewTag(name.Value);
      }

      await _context.SaveChangesAsync();

      return Result.Success();
    }

    private async Task UpdateTagWith(int id, Name name)
    {
      var tag = await _context.ProgressTags.FindAsync(id);
      tag.Update(name);
    }

    private async Task CreateNewTag(Name name)
    {
      var tag = new ProgressTag(name);
      await _context.ProgressTags.AddAsync(tag);
    }

    public record Command
    {
      public int? Id { get; set; }
      public string Name { get; set; } = null!;
    }
  }
}