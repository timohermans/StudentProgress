using System.ComponentModel;
using System.Reflection;

namespace StudentProgress.Core.Extensions;

public static class EnumExtensions
{
    public static string ToFriendlyString(this Enum value)
    {
        MemberInfo? memberInfo = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        DescriptionAttribute? attribute =
            memberInfo?.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
        return attribute?.Description ?? value.ToString();
    }
}