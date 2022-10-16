using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core;
using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.CanvasApi.Models;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using StudentProgress.Core.UseCases.Canvas.Courses;

namespace StudentProgress.Web.Pages.Canvas.Courses;

public class Details : PageModel
{
    private readonly ICanvasClient _client;
    private readonly ProgressContext _db;
    private readonly GetCourseDetailsUseCase _getUseCase;
    private readonly GroupImportFromCanvas _importUseCase;

    public IEnumerable<GetCourseDetailsResponse>? Semesters { get; set; } = null!;

    [BindProperty] public GroupImportFromCanvas.Request Semester { get; set; } = null!;
    
    public Details(ICanvasClient client, ProgressContext db, ICoreConfiguration config, HttpClient httpClient)
    {
        _getUseCase = new GetCourseDetailsUseCase(client);
        _importUseCase = new GroupImportFromCanvas(db, config, httpClient);
    }

    public async Task OnGetAsync(string id)
    {
        Semesters = await _getUseCase.Execute(id);
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await _importUseCase.Handle(Semester);
        return RedirectToPage("/Index");
    } 
}