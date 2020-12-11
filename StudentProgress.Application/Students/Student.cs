using StudentProgress.Application.Groups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StudentProgress.Application.Students
{
    public class Student
    {
        public virtual int Id { get; protected set; }
        public virtual string Name { get; protected set; }
        private readonly ICollection<Group> _groups;
        public virtual IReadOnlyList<Group> Groups => _groups.ToList();

#nullable disable
        protected Student() { }
#nullable enable


        public Student(string name)
        {
            Name = name;
            _groups = new List<Group>();
        }
    }
}
