using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentProgress.Application.Groups
{
    public class GroupMap : ClassMap<Group>
    {
        public GroupMap()
        {
            Id(g => g.Id);
            Map(g => g.Mnemonic);
            Map(g => g.Name);
            HasManyToMany(g => g.Students)
                .Cascade.All()
                .Table("StudentStudentGroup")
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
