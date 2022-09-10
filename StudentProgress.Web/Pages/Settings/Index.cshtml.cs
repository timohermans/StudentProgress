using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Web.Pages.Settings;

public class Index : PageModel
{
    private readonly SettingsGet _getUseCase;
    private readonly SettingsSet _setUseCase;

    [BindProperty] public SettingsSet.Request Settings { get; set; } = null!;

    public Index(ProgressContext db)
    {
        _getUseCase = new SettingsGet(db);
        _setUseCase = new SettingsSet(db);
    }


    public async Task OnGetAsync()
    {
        var response = await _getUseCase.HandleAsync();

        Settings = new SettingsSet.Request
        {
            CanvasApiKey = response.CanvasApiKey ?? ""
        };
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await _setUseCase.HandleAsync(Settings);

        return RedirectToPage("/Settings/Index");
    }
}