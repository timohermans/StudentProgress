﻿using CSharpFunctionalExtensions;

namespace StudentProgress.Core.Entities
{
    public class Milestone : Entity<int>
    {
        public Name LearningOutcome { get; private set; }
        public Name Artefact { get; private set; }
        public int StudentGroupId { get; }
        public StudentGroup StudentGroup { get; private set; }

        public Milestone(Name learningOutcome, Name artefact, StudentGroup studentGroup)
        {
            LearningOutcome = learningOutcome;
            Artefact = artefact;
            StudentGroup = studentGroup;
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