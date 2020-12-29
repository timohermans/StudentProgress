using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;

namespace StudentProgress.Core.Entities
{
    public class Student : Entity<int>
    {
        [Required]
        public string Name { get; private set; }
        public IEnumerable<ProgressUpdate> ProgressUpdates { get; private set; }
        public IEnumerable<StudentGroup> Groups { get; private set; }

        #nullable disable
        private Student() { }
        #nullable enable

        public Student(string name)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            ProgressUpdates = new List<ProgressUpdate>();
            Groups = new List<StudentGroup>();
        }

    }
}
