using CSharpFunctionalExtensions;
using StudentProgress.Core.UseCases;

namespace StudentProgress.Core.Entities
{
    public class Milestone : Entity<int>
    {
        public Name LearningOutcome { get; private set; }
        public Name Artefact { get; private set; }
        public StudentGroup StudentGroup { get; private set; }
        
        public Milestone(Name learningOutcome, Name artefact)
        {
            LearningOutcome = learningOutcome;
            Artefact = artefact;
        }

#nullable disable
        private Milestone()
        {
        }
#nullable enable

        public Result UpdateDetails(Name learningOutcome, Name artefact)
        {
            LearningOutcome = learningOutcome;
            Artefact = artefact;
            return Result.Success();
        }
    }
}