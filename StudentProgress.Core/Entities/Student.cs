using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentProgress.Core.Entities
{
    public class Student
    {
        public int Id { get; private set; }
        [Required]
        public string Name { get; private set; }
        public IEnumerable<ProgressUpdate> ProgressUpdates { get; private set; }
        public IEnumerable<StudentGroup> StudentGroups { get; private set; }

        #nullable disable
        private Student() { }
        #nullable enable

        public Student(string name)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            ProgressUpdates = new List<ProgressUpdate>();
            StudentGroups = new List<StudentGroup>();
        }

    }
}
