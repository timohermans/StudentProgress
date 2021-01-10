using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CSharpFunctionalExtensions;

namespace StudentProgress.Core.Entities
{
    public class StudentGroup : AuditableEntity<int>
    {
        public Name Name { get; private set; }

        public string? Mnemonic { get; private set; }

        private readonly List<Student> students = new();

        public IReadOnlyList<Student> Students => students;

        private readonly List<Milestone> _milestones = new();
        public IReadOnlyList<Milestone> Milestones => _milestones;


#nullable disable
        private StudentGroup()
        {
        }
#nullable enable

        public StudentGroup(Name name, string? mnemonic = null)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            Mnemonic = mnemonic;
            students = new List<Student>();
        }

        public Result AddStudent(Student student)
        {
            if (students == null)
                throw new InvalidOperationException("Students is null. Did you forget to include it in your query?");

            if (students.Contains(student))
                return Result.Failure("Student is already added to this group");
            
            students.Add(student ?? throw new NullReferenceException(nameof(student)));
            return Result.Success();
        }

        public Result UpdateGroup(Name name, string? mnemonic)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            Mnemonic = mnemonic;
            return Result.Success();
        }

        public Result AddMilestone(Milestone milestone)
        {
            if (_milestones == null)
                throw new InvalidOperationException("Milestones is null. Did you forget to include it in your query?");
            
            _milestones.Add(milestone ?? throw new NullReferenceException(nameof(milestone)));
            return Result.Success();
        }
    }
}