using System.ComponentModel;

namespace StudentProgress.Core.Entities;

public enum Rating
{
  [Description("😴")]
  Undefined = 1,
  [Description("😵‍💫")]
  Orienting = 2,
  [Description("🥵")]
  Beginning = 3,
  [Description("🙂")]
  Proficient = 4,
  [Description("🤩")]
  Advanced = 5
}