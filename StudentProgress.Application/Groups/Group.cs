using CSharpFunctionalExtensions;
using StudentProgress.Application.Students;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentProgress.Application.Groups
{
    public class Group
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; protected set; }

        public virtual string? Mnemonic { get; protected set; }
        protected readonly ICollection<Student> _students;
        public virtual IReadOnlyList<Student> Students => _students.ToList();

#nullable disable
        protected Group() { }
#nullable enable

        public Group(string name, string? mnemonic)
        {
            Name = name;
            Mnemonic = mnemonic;
            _students = new List<Student>();
        }

        public virtual Result AddStudent(Student student)
        {
            if (Students.Any(s => s.Name == student.Name))
            {
                return Result.Failure("Student is already added to group");
            }

            _students.Add(student);
            return Result.Success();
        }
    }
}
