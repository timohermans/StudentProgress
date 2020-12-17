using System.Collections.Generic;
using CSharpFunctionalExtensions;

namespace StudentProgress.Application.Groups
{
    public class GroupName : ValueObject
    {
        private readonly string _value;

        private GroupName(string value)
        {
            _value = value;
        }

        public static Result<GroupName> Create(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Result.Failure<GroupName>("Value is required");

            return Result.Success(new GroupName(input));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return _value;
        }

        public static implicit operator string(GroupName name)
        {
            return name._value;
        }
    }
}
