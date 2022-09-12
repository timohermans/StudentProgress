using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.CanvasApi;
using StudentProgress.Core.CanvasApi.Models;
using StudentProgress.Core.UseCases.Canvas.Courses;

namespace StudentProgress.Web.Pages.Canvas.Courses;

public class Index : PageModel
{
    private readonly GetAllUseCase _useCase;

    public IEnumerable<Course> Courses { get; private set; } = null!;

    public Index(ICanvasClient client) => _useCase = new GetAllUseCase(client);

    public async Task OnGetAsync()
    {
        Courses = await _useCase.HandleAsync();
    }
}