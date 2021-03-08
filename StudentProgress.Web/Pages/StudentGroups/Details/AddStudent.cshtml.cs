﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.StudentGroups.Details
{
  public class AddStudentModel : PageModel
  {
    private readonly ProgressContext _context;
    private readonly StudentAddToGroup _useCase;

    public StudentGroup Group { get; set; }

    public AddStudentModel(ProgressContext context)
    {
      _context = context;
      _useCase = new StudentAddToGroup(_context);
    }

    public IActionResult OnGet(int? groupId)
    {
      Group = _context.Groups.FirstOrDefault(g => g.Id == (groupId ?? 0));
      if (Group == null)
      {
        RedirectToPage("/StudentGroups/Index");
      }

      return Page();
    }

    public int? GroupId { get; set; }

    [BindProperty] public StudentAddToGroup.Request Student { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
      await AddStudentToGroup();

      if (!ModelState.IsValid) return Page();

      return RedirectToPage("./Index", new { Id = Student.GroupId });
    }

    public async Task<IActionResult> OnPostAndAddAnotherAsync()
    {
      await AddStudentToGroup();

      if (!ModelState.IsValid) return Page();

      return RedirectToPage("./AddStudent", new { Student.GroupId });
    }

    private async Task<IActionResult> AddStudentToGroup()
    {
      if (!ModelState.IsValid)
      {
        return Page();
      }

      try
      {
        await _useCase.HandleAsync(Student);
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