using CSharpFunctionalExtensions;
using StudentProgress.Application.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentProgress.Application.Common;

namespace StudentProgress.Application.Groups
{
    public class Group : AuditableEntity
    {
        public virtual int Id { get; protected set; }

        private string _name;
        public virtual GroupName Name
        {
            get => GroupName.Create(_name).Value;
            protected set => _name = value;
        }

        public virtual string? Mnemonic { get; protected set; }
        private readonly ICollection<Student> _students;
        public virtual IReadOnlyList<Student> Students => _students.ToList();

#nullable disable
        protected Group() { }
#nullable enable

        public Group(GroupName name, string? mnemonic)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
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

        public virtual Result Update(GroupName name, string? mnemonic)
        {
            Name = name;
            Mnemonic = mnemonic;

            return Result.Success();
        }
    }
}
