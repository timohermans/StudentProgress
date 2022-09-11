using System.ComponentModel.DataAnnotations;
using CSharpFunctionalExtensions;

namespace StudentProgress.Core.Entities
{
    public class Student : Entity<int>
    {
        [Required]
        public string Name { get; private set; }
        public string? AvatarPath { get; private set; }
        public IEnumerable<ProgressUpdate> ProgressUpdates { get; private set; }
        public IEnumerable<StudentGroup> StudentGroups { get; private set; }
        public string? Note { get; private set; }

#nullable disable
        private Student() { }
        #nullable enable

        public Student(string name, string? avatarPath = null)
        {
            Name = name ?? throw new NullReferenceException(nameof(name));
            AvatarPath = avatarPath;
            ProgressUpdates = new List<ProgressUpdate>();
            StudentGroups = new List<StudentGroup>();
        }

        public Result Update(string? name, string? note)
        {
            Name = name ?? Name;
            Note = note;
            return Result.Success();
        }
        
        public Result UpdateAvatar(string? avatarPath)
        {
            AvatarPath = avatarPath;
            return Result.Success();
        }
    }
}
