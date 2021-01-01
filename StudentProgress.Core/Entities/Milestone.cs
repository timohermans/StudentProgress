using CSharpFunctionalExtensions;

namespace StudentProgress.Core.Entities
{
    public class Milestone : Entity<int>
    {
        public Name Name { get; private set; }
        
        #nullable disable
        private Milestone()
        {
        }
        #nullable enable

        public Milestone(Name name)
        {
            Name = name;
        }
    }
}