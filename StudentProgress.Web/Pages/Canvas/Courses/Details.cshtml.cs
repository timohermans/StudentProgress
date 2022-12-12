using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core;
using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using StudentProgress.Core.UseCases.Canvas.Courses;

namespace StudentProgress.Web.Pages.Canvas.Courses;

public class Details : PageModel
{
    private readonly GetCourseDetailsUseCase _getUseCase;
    private readonly GroupImportFromCanvas _importUseCase;

    public IEnumerable<GetCourseDetailsResponse>? Semesters { get; set; } = null!;

    [BindProperty] public GroupImportFromCanvas.Request Semester { get; set; } = null!;
    
    public Details(ICanvasClient client, ProgressContext db, ICoreConfiguration config, HttpClient httpClient)
    {
        _getUseCase = new GetCourseDetailsUseCase(client);
        _importUseCase = new GroupImportFromCanvas(db, config, httpClient);
    }

    public async Task OnGetAsync(string id, CancellationToken token)
    {
        Semesters = (await _getUseCase.Handle(new GetCourseDetailsUseCase.Command(id), token)).Courses;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken token)
    {
        await _importUseCase.Handle(Semester, token);
        return RedirectToPage("/Index");
    } 
}