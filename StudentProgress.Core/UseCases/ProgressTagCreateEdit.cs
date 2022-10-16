using CSharpFunctionalExtensions;
using StudentProgress.Core.Entities;
using System.Threading;

namespace StudentProgress.Core.UseCases
{
  public class ProgressTagCreateEdit : IUseCaseBase<ProgressTagCreateEdit.Command, Result>
  {
    private readonly ProgressContext _context;

    public ProgressTagCreateEdit(ProgressContext context)
    {
      _context = context;
    }

    public async Task<Result> Handle(Command request, CancellationToken token)
    {
      var name = Name.Create(request.Name);

      if (name.IsFailure)
      {
        return name;
      }

      if (request.Id.HasValue)
      {
        await UpdateTagWith(request.Id.Value, name.Value, token);
      }
      else
      {
        await CreateNewTag(name.Value, token);
      }

      await _context.SaveChangesAsync(token);

      return Result.Success();
    }

    private async Task UpdateTagWith(int id, Name name, CancellationToken token)
    {
      var tag = await _context.ProgressTags.FindAsync(id);
      tag.Update(name);
    }

    private async Task CreateNewTag(Name name, CancellationToken token)
    {
      var tag = new ProgressTag(name);
      await _context.ProgressTags.AddAsync(tag, token);
    }

    public record Command : IUseCaseRequest<Result>
    {
      public int? Id { get; set; }
      public string Name { get; set; } = null!;
    }
  }
}