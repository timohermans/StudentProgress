using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StudentProgress.Web.Models
{
    public class Student
    {
        public int Id { get; private set; }
        [Required]
        public string Name { get; private set; }
        public IEnumerable<ProgressUpdate> ProgressUpdates { get; private set; }
        public IEnumerable<StudentGroup> StudentGroups { get; private set; }

        private Student() { }

        public Student(string name)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            ProgressUpdates = new List<ProgressUpdate>();
            StudentGroups = new List<StudentGroup>();
        }

    }
}
