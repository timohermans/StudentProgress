using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentProgress.Core.Entities;
using StudentProgress.Core.UseCases;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentProgress.Web.Pages.Progress
{
    public enum Rating
    {
        Undefined = 1,
        Orienting = 2,
        Beginning = 3,
        Proficient = 4,
        Advanced = 5
    }

    public class ProgressMilestone
    {
        public string LearningOutcome { get; }
        public string Artifact { get; }
        public Rating CurrentRating { get; }

        public ProgressMilestone(string learningOutcome, string artifact, Rating rating)
        {
            LearningOutcome = learningOutcome ?? throw new NullReferenceException(nameof(learningOutcome));
            Artifact = artifact ?? throw new NullReferenceException(nameof(artifact));
            CurrentRating = rating;
        }
    }
    public class IndexModel : PageModel
    {
        private readonly ProgressGetForStudentInGroup _useCase;

        public IndexModel(ProgressContext context)
        {
            _useCase = new ProgressGetForStudentInGroup(context);
        }


        // styling dummy code
        public List<ProgressMilestone> ProgressMilestones = new List<ProgressMilestone>
{
    //new ProgressMilestone("1. Feedback van stakeholders", "Compleetheid documentatie", Rating.Proficient),
    //new ProgressMilestone("1. Feedback van stakeholders", "Onderbouwing beslissingen", Rating.Proficient),
    //new ProgressMilestone("2. Samenwerking en communicatie", "Samenwerking/communicatie", Rating.Orienting),
    //new ProgressMilestone("2. Samenwerking en communicatie", "Documentatie/meetings van waarde voor stakeholder", Rating.Orienting),
    //new ProgressMilestone("2. Samenwerking en communicatie", "Regelmatig vragen feedback", Rating.Orienting),
    new ProgressMilestone("3. Specificaties en ontwerpen", "Analysedocument", Rating.Proficient),
    //new ProgressMilestone("3. Specificaties en ontwerpen", "Acceptatie testen of integratie testen", Rating.Orienting),
    new ProgressMilestone("3. Specificaties en ontwerpen", "Ontwerpdocument", Rating.Orienting),
    new ProgressMilestone("4. Onderhoudbare OO applicaties", "Herhaaldelijk feedback", Rating.Advanced),
    //new ProgressMilestone("4. Onderhoudbare OO applicaties", "Up to date houden ontwerpdocument", Rating.Orienting),
    new ProgressMilestone("4. Onderhoudbare OO applicaties", "Deployment", Rating.Undefined),
    //new ProgressMilestone("4. Onderhoudbare OO applicaties", "Error handling", Rating.Orienting),
    //new ProgressMilestone("4. Onderhoudbare OO applicaties", "Onderhoudbaar (met of zonder docs)", Rating.Orienting),
    new ProgressMilestone("4. Onderhoudbare OO applicaties", "OO principes", Rating.Beginning),
    //new ProgressMilestone("5. Algoritmiek", "Circustrein", Rating.Orienting),
    //new ProgressMilestone("5. Algoritmiek", "Containerschip", Rating.Orienting)
};

        public ProgressGetForStudentInGroup.Response Student { get; set; }

        public async Task<IActionResult> OnGetAsync(ProgressGetForStudentInGroup.Request request)
        {
            Student = await _useCase.HandleAsync(request);
            return Page();
        }
    }
}
