using CSharpFunctionalExtensions;

namespace StudentProgress.Core.Entities
{
    public class Name : ValueObject
    {
        public string Value { get; }

        private Name(string value)
        {
            Value = value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        public static implicit operator string(Name name) => name.Value;
        public static explicit operator Name(string name) => Name.Create(name).Value;

        public static Result<Name> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return Result.Failure<Name>("Name must have a value");
            }

            if (value.Length <= 2)
            {
                return Result.Failure<Name>("Name is too short");
            }

            return Result.Success(new Name(value));
        }

        public string GetFirstPart(char separator) => Value.Split(separator, StringSplitOptions.RemoveEmptyEntries).First();
    }
}