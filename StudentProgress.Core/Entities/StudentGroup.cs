using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace StudentProgress.Core.Entities
{
    public class StudentGroup : AuditableEntity
    {
        public int Id { get; private set; }
        [Required]
        public string Name { get; private set; }

        public string Mnemonic { get; private set; }
        private IList<Student> students;
        public IEnumerable<Student> Students { get => students; set => students = value.ToList(); }

        private StudentGroup() { }

        public StudentGroup(string name, string mnemonic = null)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            Mnemonic = mnemonic;
            students = new List<Student>();
        }

        public void AddStudent(Student student)
        {
            if (students == null) throw new InvalidOperationException("Students is null. Did you forget to include it in your query?");
            students.Add(student ?? throw new NullReferenceException(nameof(student)));
        }

        public void UpdateGroup(string name, string mnemonic)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            Mnemonic = mnemonic;
        }
    }
}
