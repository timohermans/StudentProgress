using FluentNHibernate.Mapping;

namespace StudentProgress.Application.Students
{
    public class StudentMap : ClassMap<Student>
    {
        public StudentMap()
        {
            Id(s => s.Id);
            Map(s => s.Name);
            HasManyToMany(s => s.Groups)
                .Cascade.All()
                .Inverse()
                .Access.CamelCaseField(Prefix.Underscore);
        }
    }
}
