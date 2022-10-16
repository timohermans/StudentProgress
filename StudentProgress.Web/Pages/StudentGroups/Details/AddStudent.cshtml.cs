using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.StudentGroups.Details
{
  public class AddStudentModel : PageModel
  {
    private readonly ProgressContext _context;
    private readonly StudentAddToGroup _useCase;

    public StudentGroup Group { get; set; } = null!;

    public AddStudentModel(ProgressContext context)
    {
      _context = context;
      _useCase = new StudentAddToGroup(_context);
    }

    public IActionResult OnGet(int? groupId)
    {
      var group = _context.Groups.FirstOrDefault(g => g.Id == (groupId ?? 0));
      if (group == null)
      {
        return NotFound();
      }

      Group = group;

      return Page();
    }

    public int? GroupId { get; set; }

    [BindProperty] public StudentAddToGroup.Request Student { get; set; } = null!;

    public async Task<IActionResult> OnPostAsync(CancellationToken token)
    {
      await AddStudentToGroup(token);

      if (!ModelState.IsValid) return Page();

      return RedirectToPage("./Index", new { Id = Student.GroupId });
    }

    public async Task<IActionResult> OnPostAndAddAnotherAsync(CancellationToken token)
    {
      await AddStudentToGroup(token);

      if (!ModelState.IsValid) return Page();

      return RedirectToPage("./AddStudent", new { Student.GroupId });
    }

    private async Task<IActionResult> AddStudentToGroup(CancellationToken token)
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      try
      {
        await _useCase.Handle(Student, token);
      }
      catch (InvalidOperationException ex)
      {
        ModelState.AddModelError("Summary", ex.Message);
        return Page();
      }

      return Page();
    }
  }
}