using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentProgress.Web.Models
{
    public class StudentGroup : AuditableEntity
    {
        public int Id { get; private set; }
        [Required]
        public string Name { get; private set; }
        private IList<Student> students;
        public IEnumerable<Student> Students { get => students; set => students = value.ToList(); }

        private StudentGroup() { }

        public StudentGroup(string name)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            students = new List<Student>();
        }

        public void AddStudent(Student student)
        {
            if (students == null) throw new InvalidOperationException("Students is null. Did you forget to include it in your query?");
            students.Add(student ?? throw new NullReferenceException(nameof(student)));
        }
    }
}
